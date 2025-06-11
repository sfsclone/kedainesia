using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    public float preAcceptPatienceTime = 15f;
    public float postAcceptPatienceTime = 20f;
    private float currentTimer;
    private bool isAccepted = false;
    private bool isServed = false;
    private CustomerManager customerManager;

    public GameObject acceptOrderButton;
    public GameObject patienceSliderUI;
    public Slider patienceSlider;

    private bool isTimerActive = true;

    [Header("Emoji")]
    public Image emojiImage;          // assign the Image component in Inspector
    public Sprite smileEmoji;         // assign smile sprite in Inspector
    public Sprite angryEmoji;         // assign angry sprite in Inspector
    public float emojiDisplayDuration = 1.5f;

    private void Start()
    {
        customerManager = FindAnyObjectByType<CustomerManager>();
        currentTimer = preAcceptPatienceTime;
        patienceSlider.maxValue = preAcceptPatienceTime;
        patienceSlider.value = currentTimer;

        if (emojiImage) emojiImage.enabled = false;  // hide emoji initially
    }

    private void Update()
    {
        if (!isTimerActive || isServed) return;

        currentTimer -= Time.deltaTime;
        patienceSlider.value = currentTimer;

        if (currentTimer <= 0)
        {
            isTimerActive = false;

            if (!isServed)
            {
                if (emojiImage)
                {
                    emojiImage.sprite = angryEmoji;
                    emojiImage.enabled = true;
                }

                FindAnyObjectByType<WarningSystem>()?.AddWarning();

                StartCoroutine(LeaveUnhappy());
            }
        }
    }

    public void OnAcceptOrder()
    {
        isAccepted = true;
        currentTimer = postAcceptPatienceTime;
        patienceSlider.maxValue = postAcceptPatienceTime;
        patienceSlider.value = currentTimer;
        acceptOrderButton.SetActive(false);
    }

    public void MarkAsServed()
    {
        isServed = true;
        isTimerActive = false;

        StartCoroutine(LeaveHappily());
    }

    private IEnumerator LeaveHappily()
    {
        if (emojiImage)
        {
            emojiImage.sprite = smileEmoji;
            emojiImage.enabled = true;
        }

        yield return new WaitForSeconds(emojiDisplayDuration);

        customerManager?.OnCustomerLeftImpatiently();
        Destroy(gameObject);
    }

    private IEnumerator LeaveUnhappy()
    {
        yield return null; // wait one frame to ensure emoji appears
        yield return new WaitForSeconds(emojiDisplayDuration);

        customerManager?.OnCustomerLeftImpatiently();
        Destroy(gameObject);
    }

    public bool IsOrderAccepted() => isAccepted;
}
