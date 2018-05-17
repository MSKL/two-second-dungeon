using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{

    public Enemy enemy;
    PlayerScript playerScript;
    TickGenerator clock;

    [HideInInspector()]
    public bool wasAttacking;
    [HideInInspector()]
    public float roundSpawned;
    Vector2 attackingPos;

    public GameObject[] corpse;

    [Header("In n rounds perform action: ")]
    public int speed = 2;

    [Header("Max health <1, maxHealth>")]
    [Range(1, 35)]
    public int maxHealth = 4;

    [Header("Restrict movement to XY only: ")]
    public bool restrictMovementToXY = false;

    [Header(("Sight: "))]
    public int sight = 8;

    [Header("Armor <0, armor>: ")]
    public int armor;

    [Header("Bare hand damage <0, bareHandDamage>: ")]
    [Range(0, 15)]
    public int bareHandDamage = 4;

    [HideInInspector()]
    public GameObject weapToEquip;


    void Start()
    {
        Initialise();

        clock = GameMap.Instance.tickGenerator;
        playerScript = GameMap.Instance.playerGameObject.GetComponent<PlayerScript>();

        if (!GameMap.Instance.enemyList.Contains(gameObject))
        {
            GameMap.Instance.enemyList.Add(gameObject);                 // !!! Přiřazení se bude provádět odjinud
        }

        roundSpawned = GameMap.Instance.player.roundsAlive;
    }

    public void Initialise()
    {
        enemy = new Enemy(transform.position, maxHealth, maxHealth, armor, sight);
        enemy.position = transform.position;
        enemy.restrictedToDiagonalMovementOnly = restrictMovementToXY;
        enemy.armor = Random.Range(0, armor + 1);
        enemy.maxHealth = Random.Range(0, maxHealth + 1);
        enemy.sight = sight;

        if (weapToEquip != null)
        {
            GameObject weapon = Instantiate(weapToEquip, Vector2.zero, Quaternion.identity) as GameObject;
            enemy.PutWeaponInHand(weapon, transform);
        }
    }

    public void OneRound()
    {
        if ((roundSpawned - GameMap.Instance.playerGameObject.GetComponent<PlayerScript>().HumanPlayer.roundsAlive) % speed == 0)       // Pohybuj se na základě rychlosti
        {

            Vector2 newPos = enemy.position + enemy.moveTowardsPlace(playerScript.HumanPlayer.position);

            if (newPos == playerScript.HumanPlayer.position)        // attack player
            {
                wasAttacking = true;
                attackingPos = newPos;
                enemy.attack(playerScript.HumanPlayer, 0, bareHandDamage+1);
            }
            else if (enemy.movingIntoEnemy(newPos))                 // moving into enemy = do nothing
            { 
                // nic nedělej
                wasAttacking = false;
            }
            else                                                    // normální pohyb
            {
                // Debug.Log(enemy.position + " " + newPos + " " + (enemy.position - newPos));
                wasAttacking = false;
                enemy.position = newPos;
            }
        }
        else
        {
            wasAttacking = false;                                   // neanimuj, pokud neútočíš
        }
    }

    void Update()
    {

        if (enemy.health <= 0) Die();                       // Pokud nemáš HP, tak umři
        else
        {
            if (wasAttacking)                               // Pokud jsi útočil, tak animuj
            {
                if (wasAttacking && clock.timer < .2F)
                {
                    transform.position = Vector2.Lerp(transform.position, (attackingPos - enemy.position) / 2 + enemy.position, .05F);
                }
                else if (wasAttacking && clock.timer < .4F)
                {
                    transform.position = Vector2.Lerp(transform.position, (attackingPos - enemy.position) / 2 + enemy.position, .05F);
                }
                else
                {
                    transform.position = Vector2.Lerp(transform.position, enemy.position, .2F);
                }
            }
            else                                            // Pokud jsi neútočil, tak se zlerpuj na finální pozici
            {
                transform.position = Vector2.Lerp(transform.position, enemy.position, Time.deltaTime * 5);
            }
        }
    }

    void Die()
    {
        enemy.isAlive = false;

        GameMap.Instance.enemyList.Remove(gameObject);                                  // Odstraň nepřítele z GameMap

        int rndNum = Random.Range(0, 3);

        if (!GameMap.Instance.objectDictionary.ContainsKey(enemy.position))             // Pokud na tomto místě nic není
        {
            switch (rndNum)
            {
                case 0:
                    if (enemy.equippedWeapon != null)
                        enemy.DropWeapon();
                    else CreateCorpse();
                    break;
                case 1:
                    CreateCorpse();
                    break;
                case 2:
                    CreateRandomDrop();
                    break;
                default:
                    Debug.Log("EnemyScript: Drop generation overflow.");
                    break;
            }
        }
        Destroy(gameObject);                                                           // Znič tento GameObject
    }

    void CreateCorpse()
    {
            GameObject instantiatedCorpse = Instantiate(corpse[Random.Range(0, corpse.Length)], enemy.position, Quaternion.identity) as GameObject;  // Vytvoř mrtvolu
            GameMap.Instance.objectDictionary.Add(enemy.position, instantiatedCorpse);
    }
    void CreateRandomDrop()
    {
        int rndNum = Random.Range(0, 3);
        GameObject inst = new GameObject();
        switch (rndNum)
        {
            case 0:
                inst = (GameObject) Instantiate(GameMap.Instance.foodPrefab[Random.Range(0, GameMap.Instance.foodPrefab.Length)], enemy.position, Quaternion.identity);
                break;
            case 1:
                inst = (GameObject)Instantiate(GameMap.Instance.armorPrefab[Random.Range(0, GameMap.Instance.armorPrefab.Length)], enemy.position, Quaternion.identity);
                break;
            case 2:
                inst = (GameObject)Instantiate(GameMap.Instance.healthPackFrefab[Random.Range(0, GameMap.Instance.foodPrefab.Length)], enemy.position, Quaternion.identity);
                break;
            default:
                Debug.Log("EnemyScript: Random drop generation overflow;");
                break;
        }
        GameMap.Instance.objectDictionary.Add(enemy.position, inst);
    }
}
