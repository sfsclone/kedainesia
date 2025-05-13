using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Cooking/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public Sprite foodIcon;
    public List<string> requiredIngredients; // Use ingredient names for now
}
