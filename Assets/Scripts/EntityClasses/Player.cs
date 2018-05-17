using UnityEngine;
using System.Collections;

public class Player : Entity {

        public float money;
        public float food;
        public float maxFood;
        public string name;

        public float decreaseFoodInTurns = 3;

        public Player(Vector2 _position, string _name, float _health, float _armor, float _food, float maxFood)
        {
            maxHealth = 100;
            position = _position;
            name = _name;
            health = _health;
            armor = _armor;
            food = _food;
        }

        public Player(Vector2 _positon)
        {
            System.Random rnd = new System.Random(System.DateTime.Now.GetHashCode());
            position = _positon;
            maxHealth = rnd.Next(13, 18);
            health = maxHealth;
            armor = rnd.Next(0, 4);
            food = rnd.Next(50, 100);
            maxFood = 100;
            name = "Unknown";
            roundsAlive = 0;
        }

        public void addHealth(float healthToAdd)
        {
            health += healthToAdd;
            if (health > maxHealth) health = maxHealth;
        }

        public void addFood(float foodToAdd)
        {
            food += foodToAdd;
            if (food > maxFood)
            {
                food = maxFood;
            } 
        }

        public void addArmor(float armorToAdd)
        {
            armor += armorToAdd;
        }

        public void riseMaxHealth(float maxHealthRiseValue)
        {
            maxHealth += maxHealthRiseValue;
            addHealth(maxHealthRiseValue);
        }
}
