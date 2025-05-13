using UnityEngine;

[CreateAssetMenu(fileName = "New Customer", menuName = "Customer/Customer Data")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public Sprite customerSprite;   // Single sprite for customer (portrait + outfit)
    public RecipeData associatedRecipe;  // Recipe associated with this customer
    public string orderedFoodName;  // The food the customer ordered
}
