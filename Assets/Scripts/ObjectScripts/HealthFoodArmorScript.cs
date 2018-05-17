using UnityEngine;
using System.Collections;

public class HealthFoodArmorScript : MonoBehaviour {

    [Header("Nutrition - <x, y>")]
    public float nutrition;
    [Range(-100, 100)]
    public int lowerNutritionBound = 0;
    [Range(-100, 100)]
    public int higherNutritionBound = 0;

   
    [Header("Health - <x, y>")]
    public float health;
    [Range(-100, 100)]
    public int lowerHealthBound = 0;
    [Range(-100, 100)]
    public int higherHealthBound = 0;

    
    [Header("Raise max health - <x, y>")]
    public float raiseMaxHealth;
    [Range(-10, 10)]
    public int lowerRaiseHealthBound = 0;
    [Range(-10, 10)]
    public int higherRaiseHealthBound = 0;

    
    [Header("Armor - <x, y>")]
    public float armor;
    [Range(-15, 15)]
    public int lowerArmorBound = 0;
    [Range(-15, 15)]
    public int higherArmorBound = 0;

    void Start()
    {
        var rnd = new System.Random();
        nutrition = rnd.Next((int)lowerNutritionBound, (int)higherNutritionBound + 1);
        health = rnd.Next((int)lowerHealthBound, (int)higherHealthBound + 1);
        armor = rnd.Next((int)lowerArmorBound, (int)higherArmorBound + 1);
        raiseMaxHealth = rnd.Next((int)lowerRaiseHealthBound, (int)higherRaiseHealthBound + 1);
    }
}
