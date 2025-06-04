using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CustomerManager : MonoBehaviour
{
    private GameClock gameClock;

    [Header("Customer Setup")]
    public List<CustomerData> allCustomerPool;
    private List<CustomerData> todaysCustomers = new List<CustomerData>();
    public Transform customerSpawnPoint;
    public GameObject customerPrefab;

    [Header("UI")]
    public TMP_Text customerProgressText;

    private int currentCustomerIndex = 0;
    private GameObject currentCustomerInstance;
    private int customersServed = 0;

    [Header("Settings")]
    public float delayBetweenCustomers = 2f;

    [Header("Dependencies")]
    [SerializeField] private WarningSystem warningSystem;

    void Start()
    {
        gameClock = FindAnyObjectByType<GameClock>();
    }

    public void StartCustomerFlow()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(3f);

        if (warningSystem != null && warningSystem.HasReachedMaxWarnings)
        {
            Debug.Log("Warning limit reached. Stopping customer flow.");
            yield break;
        }

        UpdateCustomerProgress();
        SpawnNextCustomer();
    }

    public void GenerateTodaysCustomers(int day)
    {
        todaysCustomers.Clear();
        int customerCount = Mathf.Min(1 + day, 10);

        List<CustomerData> shuffledPool = new List<CustomerData>(allCustomerPool);
        ShuffleList(shuffledPool);

        for (int i = 0; i < customerCount && i < shuffledPool.Count; i++)
        {
            CustomerData customer = ScriptableObject.Instantiate(shuffledPool[i]);
            if (customer.possibleRecipes.Count > 0)
            {
                customer.selectedRecipe = customer.possibleRecipes[Random.Range(0, customer.possibleRecipes.Count)];
            }
            todaysCustomers.Add(customer);
        }

        currentCustomerIndex = 0;
        customersServed = 0;
        Debug.Log($"All customer pool size: {allCustomerPool.Count}");
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void SpawnNextCustomer()
    {
        if (warningSystem != null && warningSystem.HasReachedMaxWarnings)
        {
            Debug.Log("Max warnings reached. No more customers will spawn.");
            return;
        }

        if (currentCustomerInstance != null)
            Destroy(currentCustomerInstance);

        if (currentCustomerIndex >= todaysCustomers.Count)
        {
            Debug.Log("All customers served today!");
            if (gameClock != null)
                gameClock.CloseRestaurantEarly();
            return;
        }

        CustomerData customer = todaysCustomers[currentCustomerIndex];
        currentCustomerInstance = Instantiate(customerPrefab, customerSpawnPoint);
        Debug.Log("Customer Spawned: " + customer.customerName);

        var image = currentCustomerInstance.transform.Find("OutfitImage")?.GetComponent<Image>();
        var nameText = currentCustomerInstance.transform.Find("NameText")?.GetComponent<TMP_Text>();
        var orderText = currentCustomerInstance.transform.Find("OrderText")?.GetComponent<TMP_Text>();

        if (image)
            image.sprite = customer.customerSprite;

        if (nameText)
            nameText.text = customer.customerName;

        if (orderText && customer.selectedRecipe != null)
            orderText.text = $"Pesan: {customer.selectedRecipe.recipeName}";
        else if (orderText)
            orderText.text = "Pesan: ???";

        UpdateCustomerProgress();
    }

    public void OnCustomerLeftImpatiently()
    {
        StartCoroutine(SpawnNextCustomerWithDelay());
    }

    public void OnFoodServed(string servedFoodName)
    {
        var expected = todaysCustomers[currentCustomerIndex].selectedRecipe?.recipeName;

        if (servedFoodName == expected)
        {
            customersServed++;
            UpdateCustomerProgress();
            StartCoroutine(SpawnNextCustomerWithDelay());
        }
        else
        {
            Debug.Log("Incorrect food served.");
        }
    }

    private IEnumerator SpawnNextCustomerWithDelay()
    {
        if (currentCustomerInstance != null)
            Destroy(currentCustomerInstance);

        yield return new WaitForSeconds(delayBetweenCustomers);

        if (warningSystem != null && warningSystem.HasReachedMaxWarnings)
        {
            Debug.Log("Customer flow stopped due to max warnings.");
            yield break;
        }

        currentCustomerIndex++;
        SpawnNextCustomer();
    }

    public string GetCurrentCustomerOrder()
    {
        if (currentCustomerIndex < todaysCustomers.Count)
            return todaysCustomers[currentCustomerIndex].selectedRecipe?.recipeName ?? "";
        return "";
    }

    public bool CheckOrder(string foodName)
    {
        return currentCustomerIndex < todaysCustomers.Count &&
               todaysCustomers[currentCustomerIndex].selectedRecipe?.recipeName == foodName;
    }

    private void UpdateCustomerProgress()
    {
        customerProgressText.text = $"{customersServed}/{todaysCustomers.Count} Customers";
    }

    public void ClearCustomers()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
