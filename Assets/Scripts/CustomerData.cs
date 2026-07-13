using UnityEngine;
using System.Collections.Generic;

public enum CustomerGender
{
    Male,
    Female
}

[CreateAssetMenu(fileName = "New Customer", menuName = "Customer/Customer Data")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public Sprite customerSprite;
    public CustomerGender gender;

    [Header("Recipe Options")]
    public List<RecipeData> possibleRecipes; // Customer can order from this list

    [HideInInspector]
    public RecipeData selectedRecipe; // The one actually chosen at runtime
}
