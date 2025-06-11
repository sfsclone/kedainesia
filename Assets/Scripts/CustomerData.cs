using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Customer", menuName = "Customer/Customer Data")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public Sprite customerSprite;

    [Header("Recipe Options")]
    public List<RecipeData> possibleRecipes; // Customer can order from this list

    [HideInInspector]
    public RecipeData selectedRecipe; // The one actually chosen at runtime
}
