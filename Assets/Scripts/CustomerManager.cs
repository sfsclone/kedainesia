using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CustomerManager : MonoBehaviour
{
    [Header("Customer Setup")]
    public List<CustomerData> todaysCustomers;
    public Transform customerSpawnPoint;
    public GameObject customerPrefab; // Contains Image + Name + Order UI

    [Header("UI")]
    public TMP_Text customerProgressText;

    private int currentCustomerIndex = 0;
    private GameObject currentCustomerInstance;
    private int customersServed = 0;

    public void StartCustomerFlow()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(3f);
        UpdateCustomerProgress();
        SpawnNextCustomer();
    }



    public void SpawnNextCustomer()
    {
        if (currentCustomerInstance != null)
            Destroy(currentCustomerInstance);

        if (currentCustomerIndex >= todaysCustomers.Count)
        {
            Debug.Log("All customers served today!");
            return;
        }

        CustomerData customer = todaysCustomers[currentCustomerIndex];
        currentCustomerInstance = Instantiate(customerPrefab, customerSpawnPoint);
        Debug.Log("Customer Spawned: " + customer.customerName);  // Debugging line

        // Debug the hierarchy of the instantiated customer
        Debug.Log("Prefab Hierarchy:");
        foreach (Transform child in currentCustomerInstance.transform)
        {
            Debug.Log("Child name: " + child.name);
        }

        // Assign visuals
        var image = currentCustomerInstance.transform.Find("OutfitImage")?.GetComponent<Image>();
        var nameText = currentCustomerInstance.transform.Find("NameText")?.GetComponent<TMP_Text>();
        var orderText = currentCustomerInstance.transform.Find("OrderText")?.GetComponent<TMP_Text>();

        if (image)
            image.sprite = customer.customerSprite;
        else
            Debug.LogWarning("Image not found!");

        if (nameText)
            nameText.text = customer.customerName;
        else
            Debug.LogWarning("NameText not found!");

        if (orderText)
            orderText.text = $"Pesan: {customer.orderedFoodName}";
        else
            Debug.LogWarning("OrderText not found!");

        UpdateCustomerProgress();
    }

    public string GetCurrentCustomerOrder()
    {
        if (currentCustomerIndex < todaysCustomers.Count)
            return todaysCustomers[currentCustomerIndex].orderedFoodName;
        return "";
    }


    public bool CheckOrder(string foodName)
    {
        return currentCustomerIndex < todaysCustomers.Count &&
               todaysCustomers[currentCustomerIndex].orderedFoodName == foodName;
    }

    public void OnFoodServed(string servedFoodName)
    {
        var expected = todaysCustomers[currentCustomerIndex].orderedFoodName;

        if (servedFoodName == expected)
        {
            customersServed++;
            currentCustomerIndex++;
            UpdateCustomerProgress();
            SpawnNextCustomer();
        }
        else
        {
            Debug.Log("Incorrect food served.");
            // Optional: Add warning or retry
        }
    }

    private void UpdateCustomerProgress()
    {
        customerProgressText.text = $"{customersServed}/{todaysCustomers.Count} Customers";
    }
}
