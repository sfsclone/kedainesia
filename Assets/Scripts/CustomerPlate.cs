using UnityEngine;
using UnityEngine.EventSystems;

public class CustomerPlate : MonoBehaviour, IDropHandler
{
    public CustomerManager customerManager;

    public void OnDrop(PointerEventData eventData)
    {
        DragCookedFood draggedFood = eventData.pointerDrag.GetComponent<DragCookedFood>();
        if (draggedFood != null)
        {
            // Check if this is the correct customer's order
            string expectedOrder = customerManager.GetCurrentCustomerOrder();

            if (draggedFood.foodName == expectedOrder)
            {
                // Successfully served
                draggedFood.ServeToCustomer(transform);
                customerManager.OnFoodServed(draggedFood.foodName);

                // Optional: Play success sound/effect
            }
            else
            {
                // Wrong order
                draggedFood.ReturnToPlate();
                // Optional: Play error sound/effect
            }
        }
    }
}