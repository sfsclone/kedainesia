using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        customerManager = FindAnyObjectByType<CustomerManager>();
        currentTimer = preAcceptPatienceTime;
        patienceSlider.maxValue = preAcceptPatienceTime;
        patienceSlider.value = currentTimer;
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
                FindAnyObjectByType<WarningSystem>()?.AddWarning();
            }

            if (customerManager != null)
            {
                customerManager.OnCustomerLeftImpatiently();
            }
            Destroy(gameObject);

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
    }

    public bool IsOrderAccepted() => isAccepted;
}
