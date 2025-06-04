using UnityEngine;
using UnityEngine.UI;

public class AcceptOrderButton : MonoBehaviour
{
    public CustomerController customer;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnAcceptClicked);
    }

    private void OnAcceptClicked()
    {
        customer.OnAcceptOrder();
    }
}
