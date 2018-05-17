using UnityEngine;
using System.Collections;

public class Entity {

    public Vector2 position;

    public float maxHealth = 100;

    public float health;
    public float armor;
    public int roundsAlive;

    public GameObject equippedWeapon;

    public bool isAlive = true; // Každá entita se rodí živá


    public bool movingIntoWall(Vector2 movingToPoint)
    {
        if (GameMap.Instance.wallDictionary.ContainsKey(movingToPoint))
        {
            return true;
        }
        else return false;
    }

    public GameObject movingOntoObject(Vector2 movingToPoint, string ObjectTagName)
    {
        if (GameMap.Instance.objectDictionary.ContainsKey(movingToPoint) && GameMap.Instance.objectDictionary[movingToPoint].name == ObjectTagName)
        {
            return GameMap.Instance.objectDictionary[position];
        }
        else
        {
            return null;
        }
    }

    public GameObject isThereAnObject(Vector2 position){
        if (GameMap.Instance.objectDictionary.ContainsKey(position))
        {
            return GameMap.Instance.objectDictionary[position];
        }
        else
        {
            return null;
        }
    }

    public GameObject movingIntoEnemy(Vector2 movingToPoint)
    {
        foreach (GameObject go in GameMap.Instance.enemyList)
        {
            if (go.gameObject == null)
            {
                GameMap.Instance.enemyList.Remove(go);
                return null;
            }
            else if (go.GetComponent<EnemyScript>().enemy.position == movingToPoint)
            {
                return go;
            }
        }
        return null;
    }

    public void PutWeaponInHand(GameObject weaponToPutInHands, Transform ownerTransform)
    {
        if (weaponToPutInHands != null)                                                           // jen pokud nějakou zbraň mám
        {
            equippedWeapon = weaponToPutInHands;                                                  // Equipnu zbraň do slotu
            equippedWeapon.transform.parent = ownerTransform;                                     // nastavím hráče jako rodičovský transform
            equippedWeapon.GetComponent<SpriteRenderer>().sortingLayerName = "ForePlayer";        // sortingLayer "ForePlayer" je před hráčem
            equippedWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 1);                     // zmenším
            equippedWeapon.transform.localPosition = new Vector3(0.167f, 0.056f, 1);              // posunu
        }
    }

    public void PickUpWeapon(GameObject weaponToPickup, Transform ownerTransform)
    {
        GameMap.Instance.objectDictionary.Remove(weaponToPickup.transform.position);    // Ostraním referenci z objectDictionary - musím udělat první!
        if (equippedWeapon != null)                                                     // Pokud hráč již má equipnutou zbraň, tak ji upustí
        {
            DropWeapon();
        }
        equippedWeapon = weaponToPickup;                                                      // Equipnu zbraň do slotu
        equippedWeapon.transform.parent = ownerTransform;                                     // nastavím hráče jako rodičovský transform
        equippedWeapon.GetComponent<SpriteRenderer>().sortingLayerName = "ForePlayer";        // sortingLayer "ForePlayer" je před hráčem
        equippedWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 1);                     // zmenším
        equippedWeapon.transform.localPosition = new Vector3(0.167f, 0.056f, 1);              // posunu
    }

    public void DropWeapon()
    {
        if (!GameMap.Instance.objectDictionary.ContainsKey(position))                                   // Pokud GameMap neobsahuje v objectdictionary key
        {
            equippedWeapon.transform.localScale = new Vector3(1, 1, 1);                                 // Změním měřítko na default
            equippedWeapon.transform.localPosition = new Vector3(0, 0, 1);                              // Změním lokální pozici na [0, 0]
            equippedWeapon.GetComponent<SpriteRenderer>().sortingLayerName = "Object";                  // Změním SortingLayer na "Object"
            equippedWeapon.transform.parent = null;                                                     // Odstraním rodičovský transform
            equippedWeapon.transform.position = position;                                               // Nastavím pozici na reálnou pozici hráče (= zaokrouhlenou)
            GameMap.Instance.objectDictionary.Add(equippedWeapon.transform.position, equippedWeapon);   // Přidám referenci do ObjectDictionary
        }
        else
        {
            Debug.Log("Entity: Trying to drop weapon, but there is already something in the objectDictionary. Destryoing the wep.");
            equippedWeapon.GetComponent<WeaponScript>().Destroy();                             // ve wepscriptu je custom funkce destroy, protože nereferencuji z MonoBehaviour
            equippedWeapon = null;                                                             // Odstarním equip hráče
        }

    }

    public void getHit(float force)
    {
        
        float damageTaken = (1 - (5 * Mathf.Sqrt(10 * armor) / 100)) * force;
        // Debug.Log("Entity: Getting hit by force: " + force + " taking " + Mathf.CeilToInt(damageTaken) + " damage.");
        health = health - Mathf.CeilToInt(damageTaken);

        if (health < 0)
        {
            isAlive = false;
        }
    }

    public void attack(Entity targetEntity, int lowBareHandAttack, int highBareHandAttack)    // tries to use weapon -> if not barehandAttack <low, high>
    {
        if (equippedWeapon != null)
        {
            targetEntity.getHit(equippedWeapon.GetComponent<WeaponScript>().damage);
        }
        else
        {
            System.Random rnd = new System.Random();
            targetEntity.getHit(rnd.Next(lowBareHandAttack, highBareHandAttack+1));
        }
    }


}
