using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    public float preAcceptPatienceTime = 15f;
    public float postAcceptPatienceTime = 60f;
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

    [Header("Gendered SFX")]
    public AudioClip happyMaleSFX;
    public AudioClip happyFemaleSFX;
    public AudioClip angryMaleSFX;
    public AudioClip angryFemaleSFX;
    private CustomerData customerData;

    public void Initialize(CustomerData data)
    {
        customerData = data;
        Debug.Log($"[CustomerController] Initialize called for customer {customerData?.customerName}, gender: {customerData?.gender}");
    }

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
                /*
                if (emojiImage)
                {
                    emojiImage.sprite = angryEmoji;
                    emojiImage.enabled = true;
                }
                */

                AudioClip clipToPlay = angrySFX;
                if (customerData != null)
                {
                    if (customerData.gender == CustomerGender.Female && angryFemaleSFX != null)
                        clipToPlay = angryFemaleSFX;
                    else if (customerData.gender == CustomerGender.Male && angryMaleSFX != null)
                        clipToPlay = angryMaleSFX;
                }

                Debug.Log($"[CustomerController] Unhappy timeout. Customer: {customerData?.customerName}, Gender: {customerData?.gender}, Clip to play: {clipToPlay?.name}");

                if (AudioManager.Instance != null && clipToPlay != null)
                {
                    AudioManager.Instance.PlaySFX(clipToPlay);
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

        AudioClip clipToPlay = happySFX;
        if (customerData != null)
        {
            if (customerData.gender == CustomerGender.Female && happyFemaleSFX != null)
                clipToPlay = happyFemaleSFX;
            else if (customerData.gender == CustomerGender.Male && happyMaleSFX != null)
                clipToPlay = happyMaleSFX;
        }

        Debug.Log($"[CustomerController] LeaveHappily. Customer: {customerData?.customerName}, Gender: {customerData?.gender}, Clip to play: {clipToPlay?.name}");

        if (AudioManager.Instance != null && clipToPlay != null)
        {
            AudioManager.Instance.PlaySFX(clipToPlay);
        }

        yield return new WaitForSeconds(emojiDisplayDuration);

        customerManager?.OnCustomerLeftImpatiently();
        Destroy(gameObject);
    }

    private IEnumerator LeaveUnhappy()
    {
        Transform outfitTransform = transform.Find("OutfitImage");
        Image outfitImage = outfitTransform != null ? outfitTransform.GetComponent<Image>() : null;
        Color originalColor = Color.white;
        if (outfitImage != null)
        { 
            originalColor = outfitImage.color;
            Color flashColor;
            if (ColorUtility.TryParseHtmlString("#FF745C", out flashColor))
            {
                outfitImage.color = flashColor;
            }
            else
            {
                outfitImage.color = new Color32(0xFF, 0x4D, 0x2E, 0xFF);
            }
        }

        yield return new WaitForSeconds(emojiDisplayDuration);

        if (outfitImage != null)
        {
            outfitImage.color = originalColor;
        }

        customerManager?.OnCustomerLeftImpatiently();
        Destroy(gameObject);
    }

    public bool IsOrderAccepted() => isAccepted;
}
