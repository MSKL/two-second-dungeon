using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class PlayerScript : MonoBehaviour
{

    public Player HumanPlayer;
    public TickGenerator clock;

    [Header("Bare hand damage <0, i)")]
    [Range(0, 10)]
    public float bareHandDamage = 3;

    void Start()
    {
        clock = GameMap.Instance.tickGenerator;
        clock.PlayerScript = this;
    }

    // GameObjecty HUDu
    GameObject sipka;
    public GameObject sipkaPrefab;
    GameObject mec;
    public GameObject mecPrefab;
    GameObject smallCross;
    public GameObject smallCrossPrefab;
    GameObject InteractNotifier;
    public GameObject InteractNotifierPrefab;

    public GameObject playerCorpsePrefab;

    Vector2 movingTo = new Vector2(0, 0);

    bool movingIntoEnemy = false;
    bool movingIntoWall = false;

    void Update() {

        CheckIfDead();

        // ----- Nejprve zkontroluj uživatelský vstup a přiřaď hodnotu do movingTo -----

        HandleInput();

        // ----- Zde začínají checky na booleany MovingInto -----

        movingIntoWall = false;
        if (HumanPlayer.movingIntoWall(HumanPlayer.position + movingTo) == true)
        {
            movingIntoWall = true;
        }

        movingIntoEnemy = false;
        if (HumanPlayer.movingIntoEnemy(HumanPlayer.position + movingTo) == true)
        {
            movingIntoEnemy = true;
        }

        // ----- Zde začínají checky na spawnování HUDu -----

        SpawnIngameHUD();

        GameObject standingOnGameObject = HumanPlayer.isThereAnObject(HumanPlayer.position);
        if (standingOnGameObject != null)                               // Pokud stojím nad nějakým GameObjectem
        {
            if (InteractNotifier == null)                               // Pokud neexistuje ještě instance InteractNotifier
            {                                                           // Instancuj a posuň o hodnotu.. (hardcoded)
                InteractNotifier = Instantiate(InteractNotifierPrefab, transform.position + new Vector3(-0.629F, 0.522F, 0), Quaternion.identity) as GameObject;
                InteractNotifier.transform.parent = this.transform;     // Nastav hráče jako parent
            }

            if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                if (standingOnGameObject.tag == "StairsDown")
                {
                    GameMap.Instance.loadNextLevel();
                }
                
                if (standingOnGameObject.tag == "Weapon")       // Pokud studíš na GameObjectu s tagem Weapon, tak ho seber a equipni si ho do slotu
                {
                    HumanPlayer.PickUpWeapon(standingOnGameObject, transform);
                }
                else if (standingOnGameObject.tag == "HealthFoodArmor")
                {
                    HealthFoodArmorScript hfa = standingOnGameObject.GetComponent<HealthFoodArmorScript>();
                    float healthToAdd = hfa.health;
                    float riseMaxHealth = hfa.raiseMaxHealth;
                    float armorToAdd = hfa.armor;
                    float foodToAdd = hfa.nutrition;

                    HumanPlayer.riseMaxHealth(riseMaxHealth);
                    HumanPlayer.addHealth(healthToAdd);
                    HumanPlayer.addFood(foodToAdd);
                    HumanPlayer.addArmor(armorToAdd);

                    GameMap.Instance.objectDictionary.Remove(HumanPlayer.position);
                    Destroy(standingOnGameObject);
                }
            }
        }
        else
        {
            if (InteractNotifier != null)
            {
                Destroy(InteractNotifier);
            }
        }
    }

    bool wasAttacking = false;
    Vector2 wasAttackingToPos = Vector2.zero;


    public void OneRound()                  // Tahle metoda se volá jednou za kolo, provádí se v ní všechny kroky spojené s jedním kolem
    {
        HumanPlayer.roundsAlive++;
        if (HumanPlayer.roundsAlive % HumanPlayer.decreaseFoodInTurns == 0)
        {
            HumanPlayer.food--;
            if (HumanPlayer.food < 0)
            {
                HumanPlayer.food = 0;
                HumanPlayer.health -= 1;
            }
        }

        wasAttacking = movingIntoEnemy;     // Ukládá hodnoty o minulém kole -> Výpočty (animace pohybu) se vlastně provádí až v kole následujícím
        if (wasAttacking)
        {
            wasAttackingToPos = HumanPlayer.position + movingTo; // 1.5F;

            if (HumanPlayer.equippedWeapon != null)
            {
                HumanPlayer.movingIntoEnemy(wasAttackingToPos).GetComponent<EnemyScript>().enemy.getHit(HumanPlayer.equippedWeapon.GetComponent<WeaponScript>().damage);
            }
            else
            {
                float randomForce = Random.Range(1, 3); // Bare hands mají Attack <0, 2>
                HumanPlayer.movingIntoEnemy(wasAttackingToPos).GetComponent<EnemyScript>().enemy.getHit(randomForce);    
            }
            
        }
        else wasAttackingToPos = Vector2.zero;

        if (movingIntoEnemy == false && movingIntoWall == false)
        {
            HumanPlayer.position += movingTo;
        }
        movingTo = Vector2.zero;

    }

    // Proč vlastně používám fixedUpdate? -> Chtělo by to v budoucnu zkusit to narvat do Update() -> Možná by to zlepšilo performance
    // EDIT: Vyzkoušeno, rozdíl je asi 3fps, nestojí to za to
    void FixedUpdate() {

        if (wasAttacking && clock.timer < .2F)
        {
            transform.position = Vector2.Lerp(transform.position, wasAttackingToPos, .05F);
        }
        else if (wasAttacking && clock.timer < .4F)
        {
            transform.position = Vector2.Lerp(transform.position, HumanPlayer.position, .05F);
        }
        else
        {
            transform.position = Vector2.Lerp(transform.position, HumanPlayer.position, .2F);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.Keypad8))
        {
            if (movingTo.y < 1)
            {
                movingTo.y++;
            }
            else
            {
                clock.NextRound();
            }
        }

        else if (Input.GetKeyDown("down") || Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (movingTo.y > -1)
            {
                movingTo.y--;
            }
            else
            {
                clock.NextRound();
            }
        }
        else if (Input.GetKeyDown("left") || Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (movingTo.x > -1)
            {
                movingTo.x--;
            }
            else
            {
                clock.NextRound();
            }
        }
        else if (Input.GetKeyDown("right") || Input.GetKeyDown(KeyCode.Keypad6))
        {
            if (movingTo.x < 1)
            {
                movingTo.x++;
            }
            else
            {
                clock.NextRound();
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            if (movingTo.x < 1 && movingTo.y < 1)
            {
                movingTo.x++;
                movingTo.y++;
            }
            else
            {
                clock.NextRound();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            if (movingTo.x > -1 && movingTo.y < 1)
            {
                movingTo.x--;
                movingTo.y++;
            }
            else
            {
                clock.NextRound();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (movingTo.x > -1 && movingTo.y > -1)
            {
                movingTo.x--;
                movingTo.y--;
            }
            else
            {
                clock.NextRound();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (movingTo.x < 1 && movingTo.y > -1)
            {
                movingTo.x++;
                movingTo.y--;
            }
            else
            {
                clock.NextRound();
            }
        }

        if (Input.GetKeyDown(KeyCode.X)) // Skip round with X
        {
            clock.NextRound();
        }

    }

    void SpawnIngameHUD()
    {
        if (movingTo != Vector2.zero)   // Pokud se pohybuji kamkoliv
        {
            Destroy(sipka);             // Teoreticky bych toto nemusel dělat, kdybych zkontroloval kde aktuální šipka je, ale snížilo by to o dost čitelnost kódu
            sipka = Instantiate(sipkaPrefab, new Vector3(HumanPlayer.position.x, HumanPlayer.position.y) + new Vector3(movingTo.x / 2, movingTo.y / 2), Quaternion.AngleAxis(Mathf.Atan2(-movingTo.x, movingTo.y) * Mathf.Rad2Deg, Vector3.forward)) as GameObject;
        }
        else
        {
            if (sipka != null)
            {
                Destroy(sipka);
            }
        }

        if (movingIntoWall)         // Pokud se pohybuji na políčko se zdí
        {
            Destroy(smallCross);
            smallCross = Instantiate(smallCrossPrefab, new Vector3(HumanPlayer.position.x, HumanPlayer.position.y) + new Vector3(movingTo.x / 2, movingTo.y / 2), Quaternion.AngleAxis(Mathf.Atan2(-movingTo.x, movingTo.y) * Mathf.Rad2Deg, Vector3.forward)) as GameObject;
        }
        else                        // Pokud se na políčko se zdí nepohybuji
        {
            if (smallCross != null) // Pokud se pohybuji na políčko, kde není zeď, ale mám spawnutý křížek, tak je potřeba ho zničit
            {
                Destroy(smallCross);
            }
        }

        if (movingIntoEnemy)        // Pokud se pohybuji na políčko, kde je nepřítel
        {
            if (mec == null)
            {
                mec = Instantiate(mecPrefab, new Vector3(HumanPlayer.position.x, HumanPlayer.position.y) + new Vector3(movingTo.x, movingTo.y), Quaternion.identity) as GameObject;
            }
        }
        else                        // Pokud se na políčko s nepřítelem nepohybuji
        {
            if (mec != null)
            {
                Destroy(mec);
            }
        }
    }

    void CheckIfDead()      // Zkontroluj jestli není hráč mrtvý
    {
        if (HumanPlayer.health <= 0)
        {
            GameMap.Instance.tickGenerator.enabled = false;

            int i = 0;
            while (PlayerPrefs.HasKey(i.ToString()))
            {
                i += 2;
            }

            PlayerPrefs.SetFloat(i.ToString(), HumanPlayer.roundsAlive);            // sudé i = skóre
            PlayerPrefs.SetString((i + 1).ToString(), HumanPlayer.name);            // liché i = (i+1) = jméno
            PlayerPrefs.Save();

            GameMap.Instance.HUDCanvas.GetComponent<MainGameDeathDialogScript>().showDeathPanel(HumanPlayer.roundsAlive);
            Debug.Log("PlayerScript: Player died!");
            Destroy(gameObject);
            Instantiate(playerCorpsePrefab, HumanPlayer.position, Quaternion.identity);
        }
    }

    void OnGUI()
    {
        // GUI.Label(new Rect(30, 30, 200, 200), "Player pos: " + HumanPlayer.position + " \nPlayer health: " + HumanPlayer.health + " \nPlayer money:" + HumanPlayer.money + " \nPlayer armor: " + HumanPlayer.armor + " \nPlayer attack: " + HumanPlayer.attack + " \nPlayer food: " + HumanPlayer.food);

        GameMap.Instance.healthSlider.normalizedValue = HumanPlayer.health / HumanPlayer.maxHealth;
        GameMap.Instance.timeSlider.normalizedValue = clock.timer / clock.interval;
        GameMap.Instance.foodSlider.normalizedValue = HumanPlayer.food / HumanPlayer.maxFood;

        GameMap.Instance.healthText.text = "Život: " + HumanPlayer.health + "/" + HumanPlayer.maxHealth;
        GameMap.Instance.roundText.text = "Kolo: " + HumanPlayer.roundsAlive + "/" + GameMap.Instance.level;
        GameMap.Instance.foodText.text = "Jídlo: " + HumanPlayer.food;
        GameMap.Instance.nameText.text = "Jméno: " + HumanPlayer.name;
        GameMap.Instance.armorText.text = "Brnění: " + HumanPlayer.armor;
        if (HumanPlayer.equippedWeapon != null)
        {
            WeaponScript ws = HumanPlayer.equippedWeapon.GetComponent<WeaponScript>();
            GameMap.Instance.weaponNameText.text = "Zbraň: " + ws.name.ToString();
            GameMap.Instance.damageText.text = "Útok: " + ws.damage.ToString();
        }
        else
        {
            GameMap.Instance.weaponNameText.text = "Zbraň: " + "Pěsti";
            GameMap.Instance.damageText.text = "Útok: " + "<0, " + bareHandDamage + ")";
        }

        
                //        GameMap.Instance.healthText.text = "Health: " + HumanPlayer.health + "/" + HumanPlayer.maxHealth;
        //GameMap.Instance.roundText.text = "Round: " + HumanPlayer.roundsAlive + "/" + GameMap.Instance.level;
        //GameMap.Instance.foodText.text = "Food: " + HumanPlayer.food;
        //GameMap.Instance.nameText.text = "Name: " + HumanPlayer.name;
        //GameMap.Instance.armorText.text = "Armor: " + HumanPlayer.armor;
        //if (HumanPlayer.equippedWeapon != null)
        //{
        //    WeaponScript ws = HumanPlayer.equippedWeapon.GetComponent<WeaponScript>();
        //    GameMap.Instance.weaponNameText.text = "Weapon: " + ws.name.ToString();
        //    GameMap.Instance.damageText.text = "Damage: " + ws.damage.ToString();
        //}
        //else
        //{
        //    GameMap.Instance.weaponNameText.text = "Weapon: " + "Bare Hands";
        //    GameMap.Instance.damageText.text = "Damage: " + "<0, " + bareHandDamage + ")";
        //} // English translation // English translation

    }

    }