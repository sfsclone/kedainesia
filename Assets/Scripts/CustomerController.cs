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
    public GameObject komporButton;
    public GameObject patienceSliderUI;
    public Slider patienceSlider;

    private CraftingManager craftingManager;
    private bool isTimerActive = true;

    [Header("SFX")]
    public AudioClip patienceWarningSFX;
    public AudioClip happySFX;
    public AudioClip angrySFX;
    public float warningThreshold = 5f;
    private bool hasPlayedWarningSFX = false;

    [Header("Emoji")]
public Image emojiImage;          // assign the Image component in Inspector
    public Sprite smileEmoji;         // assign smile sprite in Inspector
    public Sprite angryEmoji;         // assign angry sprite in Inspector
    public float emojiDisplayDuration = 1.5f;

    private void Start()
    {
        customerManager = FindAnyObjectByType<CustomerManager>();
        craftingManager = FindAnyObjectByType<CraftingManager>();
        currentTimer = preAcceptPatienceTime;
        patienceSlider.maxValue = preAcceptPatienceTime;
        patienceSlider.value = currentTimer;

        if (emojiImage) emojiImage.enabled = false;  // hide emoji initially
        if (komporButton) komporButton.SetActive(false);
    }

    private void Update()
    {
        if (!isTimerActive || isServed) return;

        currentTimer -= Time.deltaTime;
        patienceSlider.value = currentTimer;

        if (currentTimer <= warningThreshold && !hasPlayedWarningSFX)
        {
            hasPlayedWarningSFX = true;
            if (AudioManager.Instance != null && patienceWarningSFX != null)
            {
                AudioManager.Instance.PlaySFX(patienceWarningSFX, true); // Added duckMusic: true
            }
        }

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

                if (AudioManager.Instance != null && angrySFX != null)
                {
                    AudioManager.Instance.PlaySFX(angrySFX);
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
        if (komporButton) komporButton.SetActive(true);
        hasPlayedWarningSFX = false;
    }

    public void OpenStove()
    {
        if (craftingManager != null)
        {
            craftingManager.OpenCraftingPanel();
        }
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

        if (AudioManager.Instance != null && happySFX != null)
        {
            AudioManager.Instance.PlaySFX(happySFX);
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
