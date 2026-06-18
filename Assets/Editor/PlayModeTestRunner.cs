using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Unity.AI.Assistant.PlayModeTest
{
    [InitializeOnLoad]
    internal static class PlayModeTestRunner
    {
        private const string StateKey = "PlayModeTest.State";
        private const string ResultKey = "PlayModeTest.Result";
        private const string ScriptPathKey = "PlayModeTest.ScriptPath";
        private const string SentinelLog = "PLAY_MODE_TEST_COMPLETE";

        private static readonly int WaitFrames = SessionState.GetInt("PlayModeTest.WaitFrames", 10);
        private static readonly float TestTimeout = SessionState.GetFloat("PlayModeTest.TestTimeout", 15.0f);

        private static List<string> _capturedLogs = new List<string>();
        private const int MaxCapturedLogs = 50;

        static PlayModeTestRunner()
        {
            string state = SessionState.GetString(StateKey, "Idle");

            switch (state)
            {
                case "Idle":
                    break;

                case "WaitingForCompile":
                    Debug.Log("[PlayModeTest] Bootstrap compiled. Scheduling Play Mode entry.");
                    EditorApplication.delayCall += () =>
                    {
                        SessionState.SetString(StateKey, "EnteringPlayMode");
                        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                        EditorApplication.isPlaying = true;
                    };
                    break;

                case "EnteringPlayMode":
                    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                    if (EditorApplication.isPlaying)
                    {
                        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                        SessionState.SetString(StateKey, "InPlayMode");
                        EditorApplication.update += WaitFramesThenRun;
                    }
                    break;

                case "InPlayMode":
                    if (EditorApplication.isPlaying)
                    {
                        EditorApplication.update += WaitFramesThenRun;
                    }
                    break;

                case "Done":
                    Debug.Log(SentinelLog);
                    EditorApplication.delayCall += SelfDestruct;
                    break;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                SessionState.SetString(StateKey, "InPlayMode");
                EditorApplication.update += WaitFramesThenRun;
            }
        }

        private static int _frameCount = 0;
        private static bool _setupDone = false;
        private static bool _testDone = false;
        private static double _testStartTime = 0;

        private static void WaitFramesThenRun()
        {
            _frameCount++;
            if (_frameCount < WaitFrames) return;

            if (_testDone) return;

            if (!_setupDone)
            {
                _setupDone = true;
                Application.logMessageReceived += OnLogMessage;
                _testStartTime = EditorApplication.timeSinceStartup;
                try
                {
                    Setup();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("[PlayModeTest] Setup threw exception: " + e);
                    FinishTest(true, e.Message);
                    return;
                }
                return;
            }

            float elapsed = (float)(EditorApplication.timeSinceStartup - _testStartTime);
            bool timedOut = elapsed >= TestTimeout;

            try
            {
                bool complete = Tick(elapsed);
                if (complete || timedOut)
                {
                    FinishTest(timedOut && !complete, timedOut ? "Test timed out after " + TestTimeout + "s" : null);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("[PlayModeTest] Tick threw exception: " + e);
                FinishTest(true, e.Message);
            }
        }

        private static void FinishTest(bool isError, string errorMessage)
        {
            _testDone = true;
            EditorApplication.update -= WaitFramesThenRun;
            Application.logMessageReceived -= OnLogMessage;

            string resultJson = GetResult();

            if (isError && errorMessage != null)
            {
                resultJson = JsonUtility.ToJson(new TestResult
                {
                    success = false,
                    error = errorMessage,
                    logs = _capturedLogs.ToArray()
                });
            }

            SessionState.SetString(ResultKey, resultJson);
            SessionState.SetString(StateKey, "Done");
            EditorApplication.isPlaying = false;
        }

        private static void OnLogMessage(string message, string stackTrace, LogType type)
        {
            if (_capturedLogs.Count >= MaxCapturedLogs) return;
            if (type == LogType.Error || type == LogType.Exception ||
                message.Contains("[Test]") || message.Contains("TEST_RESULT"))
            {
                _capturedLogs.Add("[" + type + "] " + message);
            }
        }

        private static void SelfDestruct()
        {
            string scriptPath = SessionState.GetString(ScriptPathKey, "");
            if (!string.IsNullOrEmpty(scriptPath) && AssetDatabase.AssetPathExists(scriptPath))
            {
                AssetDatabase.DeleteAsset(scriptPath);
            }
            SessionState.EraseString(StateKey);
            SessionState.EraseString(ScriptPathKey);
        }

        [System.Serializable]
        private class TestResult
        {
            public bool success;
            public string error;
            public string[] logs;
            public bool sfx_triggered;
        }

        private static void Setup()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene1")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene1");
            }
            Debug.Log("[Test] Setup: Scene GameScene1 requested");
        }

        private static bool Tick(float elapsed)
        {
            var managerObj = GameObject.Find("CustomerManager");
            if (managerObj == null) return false;

            var manager = managerObj.GetComponent("CustomerManager");
            if (manager == null) return false;

            // Trigger customer flow
            var generateMethod = manager.GetType().GetMethod("GenerateTodaysCustomers");
            var startFlowMethod = manager.GetType().GetMethod("StartCustomerFlow");

            if (generateMethod != null && startFlowMethod != null)
            {
                var todaysField = manager.GetType().GetField("todaysCustomers", (System.Reflection.BindingFlags)36);
                var todaysList = todaysField?.GetValue(manager) as System.Collections.IList;
                if (todaysList == null || todaysList.Count == 0)
                {
                    Debug.Log("[Test] Generating 1st day customers and starting flow");
                    generateMethod.Invoke(manager, new object[] { 1 });
                    startFlowMethod.Invoke(manager, null);
                }
            }

            // Find instance
            var instanceField = manager.GetType().GetField("currentCustomerInstance", (System.Reflection.BindingFlags)36);
            var customerInstance = instanceField?.GetValue(manager) as GameObject;

            if (customerInstance == null) return false;

            var controller = customerInstance.GetComponent("CustomerController");
            if (controller == null) return false;

            // Force timer to warning threshold
            var timerField = controller.GetType().GetField("currentTimer", (System.Reflection.BindingFlags)36);
            var thresholdField = controller.GetType().GetField("warningThreshold");
            var thresholdValue = (float)thresholdField.GetValue(controller);
            float currentVal = (float)timerField.GetValue(controller);

            if (currentVal > thresholdValue + 0.1f)
            {
                Debug.Log("[Test] Fast-forwarding timer to " + (thresholdValue + 0.05f));
                timerField.SetValue(controller, thresholdValue + 0.05f);
            }

            // Check if triggered
            var playedField = controller.GetType().GetField("hasPlayedWarningSFX");
            bool played = (bool)playedField.GetValue(controller);

            if (played)
            {
                Debug.Log("[Test] hasPlayedWarningSFX is true!");
                return true;
            }

            return false;
        }

        private static string GetResult()
        {
            var managerObj = GameObject.Find("CustomerManager");
            var manager = managerObj?.GetComponent("CustomerManager");
            var instanceField = manager?.GetType().GetField("currentCustomerInstance", (System.Reflection.BindingFlags)36);
            var customerInstance = instanceField?.GetValue(manager) as GameObject;
            var controller = customerInstance?.GetComponent("CustomerController");
            var playedField = controller?.GetType().GetField("hasPlayedWarningSFX");
            bool played = playedField != null && (bool)playedField.GetValue(controller);

            return JsonUtility.ToJson(new TestResult
            {
                success = played,
                sfx_triggered = played,
                logs = _capturedLogs.ToArray()
            });
        }
    }
}
