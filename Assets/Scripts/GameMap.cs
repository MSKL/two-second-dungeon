using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMap : MonoBehaviour {

    /*/////////////////////////////////////////////////////////////
     * 
     * Tento skript obsahuje reference na většinu objektů ve hře
     * 
     */////////////////////////////////////////////////////////////

    public Dictionary<Vector2, GameObject> objectDictionary;
    public List<GameObject> enemyList;
    public Dictionary<Vector2, GameObject> wallDictionary;
    public Dictionary<Vector2, GameObject> dungeonGroundDictionary;
    public Dictionary<Vector2, GameObject> corridorGroundDictionary;

    public TickGenerator tickGenerator;
    public GameObject tickGeneratorPrefab;

    [HideInInspector()]
    public int level = 1;

    public int endLevel = 14;                                                           // Hra končí v levelu 13, poslední level je naskriptovaný = 14

    [Space(10)]
    [Header("Generace Mapy")]
    public MapDungeon mapa;
    public int width = 30;                                                              // Používat sudá čísla aby w a h / 2 vyšlo celé!!
    public int height = 30;
    [Range(0, 100)]
    public int generationChanceInPct = 50;                                              // Šance na generaci políčka před celulárním automatem
    [Range(0, 10)]
    public int numOfIterations = 5;                                                     // Počet iterací celulárního automatu
    [Space(10)]

    [Header("Prefaby prostředí")]
    public GameObject[] floorPrefab;
    public GameObject[] wallPrefab;
    public GameObject exitPrefab;

    public GameObject[] foodPrefab;
    public GameObject[] armorPrefab;
    public GameObject[] healthPackFrefab;

    GameObject Enviroment;

    [Header("UI Objekty")]
    public Slider timeSlider;
    public Text roundText;
    public Slider healthSlider;
    public Text healthText;
    public Slider foodSlider;
    public Text foodText;
    public Text nameText;
    public Text armorText;
    public Text weaponNameText;
    public Text damageText;
    public Image interactiveNotifier;
    public Canvas HUDCanvas;
    [Space(10)]

    [Header("Prefaby na zbraně")]
    public GameObject branchSwordPrefab;
    public GameObject kingSlayerPrefab;
    public GameObject longSordPrefab;
    public GameObject steelStickPrefab;
    public GameObject waterSwordPrefab;
    public GameObject[] weaponPrefabsSorted;
    [Space(10)]

    [Header("Enemy prefabs: ")]
    public GameObject zombiePrefab;
    public GameObject blobPrefab;
    public GameObject skeletonPrefab;

    public Player player;
    public GameObject playerGameObject;
    public GameObject playerPrefab;

    private static GameMap _instance;   // soukromá instance singletonu
    public static GameMap Instance      // veřejná instance singletonu
    {
        get 
        {
            return _instance;           // Po zavolání get vrať tuto instanci
        }
    }

    void Awake()                        // Všechen obsah z CreateScene() se provádí ještě před voláním funkcí Start() ostatních GameObjectů
    {
        _instance = this;               // dosaď tento skript do static instance

        DontDestroyOnLoad(this);            

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        CreateScene();
        SetPlayerName();
        generateDrops();
        CreateEnemies();
    }

    public void loadNextLevel()
    {
        DestroyScene();
        level++;

        if (level == endLevel)
        {
            int i = 0;                                                          // Ulož skóre                             
            while (PlayerPrefs.HasKey(i.ToString()))
            {
                i += 2;
            }

            PlayerPrefs.SetFloat(i.ToString(), player.roundsAlive); 
            PlayerPrefs.SetString((i + 1).ToString(), player.name);             // liché i = (i+1) = jméno
            PlayerPrefs.Save();

            SceneManager.LoadScene("EndScene");
            Destroy(gameObject);
        }
        else
        {
            CreateScene();
            generateDrops();
            CreateEnemies();
        }
    }

    void CreateScene()
    {
        Debug.Log("GameMap: Creating scene for level: " + level);
        System.Random rnd = new System.Random();

                // Tomuhle nerozumím, v exploreru se mi nějak kupí GameObjecty
        //if (Enviroment != null) 
        //{
        //    Destroy(Enviroment);
        //}

        if (Enviroment == null)
        {
            Enviroment = (GameObject)Instantiate(new GameObject("Enviroment"), Vector3.zero, Quaternion.identity);  // Vytvoř GO Enviroment, který slouží spíše jako složka         
        }
           
        mapa = new MapDungeon(width, height, Random.Range(8, 10), new Vector2(3, 3), new Vector2(9, 9));            // Vygeneruj mapu se zadanými parametry

        if (tickGenerator == null)                                                                          // Zkontroluj, jestli existuje ve scéně tickGenerator
        {
            if (GameObject.Find("TickGenerator") == null)                                                   // Pokud neexistuje, tak ho instancuj
            {
                GameObject TickGeneratorGameObject = Instantiate(tickGeneratorPrefab) as GameObject;
                TickGeneratorGameObject.name = "TickGenerator";
                tickGenerator = TickGeneratorGameObject.GetComponent<TickGenerator>();
            }
            else                                                                                            // Pokud existuje, tak ho najdi
            {
                tickGenerator = GameObject.Find("TickGenerator").GetComponent<TickGenerator>();
            }
        }
        

        corridorGroundDictionary = new Dictionary<Vector2, GameObject>();                                   // Instancuj slovníky
        dungeonGroundDictionary = new Dictionary<Vector2, GameObject>();
        objectDictionary = new Dictionary<Vector2, GameObject>();
        wallDictionary = new Dictionary<Vector2, GameObject>();

        for (int x = 0; x < width; x++)                                                         // Zde probíhá instancování všech GameObjectů na základě dat z mapy
        {                                                                                       // Objekty se zrovna přidávají do slovníků
            for (int y = 0; y < height; y++)
            {
                switch (mapa.mapArray[x, y])
                {
                    case 0:     // floors
                        GameObject fPrefab = Instantiate(floorPrefab[rnd.Next(0, floorPrefab.Length - 1)], new Vector3(x, y), Quaternion.identity) as GameObject;
                        fPrefab.transform.parent = Enviroment.transform;
                        dungeonGroundDictionary.Add(fPrefab.transform.position, fPrefab);
                        break;

                    case 2:     // corridor
                        GameObject cPrefab = Instantiate(floorPrefab[rnd.Next(0, floorPrefab.Length - 1)], new Vector3(x, y), Quaternion.identity) as GameObject;
                        cPrefab.transform.parent = Enviroment.transform;
                        corridorGroundDictionary.Add(cPrefab.transform.position, cPrefab);
                        break;

                    case 1:     // wall
                        int wallToInstantiate = 0;

                        if (d(100))
                        {
                            wallToInstantiate = rnd.Next(0, wallPrefab.Length - 1);
                        }

                        GameObject wPrefab = Instantiate(wallPrefab[wallToInstantiate], new Vector3(x, y), Quaternion.identity) as GameObject;
                        wPrefab.transform.parent = Enviroment.transform;
                        wallDictionary.Add(wPrefab.transform.position, wPrefab);
                        break;

                    case 333:   // stairs
                        GameObject object_stairs = Instantiate(exitPrefab, new Vector2(x, y), Quaternion.identity) as GameObject;
                        object_stairs.transform.parent = Enviroment.transform;
                        object_stairs.name = "object_stairs";
                        objectDictionary.Add(object_stairs.transform.position, object_stairs);
                        break;

                    default:    // všechno ostatní
                        Debug.Log("The GameMap is trying to instantiate thing id " + mapa.mapArray[x, y] + " at x: " + x + " y: " + y + " but there is no prefab set.");
                        break;
                }
            }
        }

        for (int x = -4; x < (width + 4); x++)          // Nakresli spodní a horní okraje zdí
        {
            for (int y = -4; y < 0; y++)
            {
                GameObject wPrefab = Instantiate(wallPrefab[rnd.Next(0, wallPrefab.Length - 1)], new Vector3(x, y), Quaternion.identity) as GameObject;
                wPrefab.transform.parent = Enviroment.transform;
                wallDictionary.Add(wPrefab.transform.position, wPrefab);
            }
            for (int y = height; y < (height + 4); y++)
            {
                GameObject wPrefab = Instantiate(wallPrefab[rnd.Next(0, wallPrefab.Length - 1)], new Vector3(x, y), Quaternion.identity) as GameObject;
                wPrefab.transform.parent = Enviroment.transform;
                wallDictionary.Add(wPrefab.transform.position, wPrefab);
            }
        }

        for (int y = 0; y < (height); y++)              // Levý a pravý okraj
        {
            for (int x = -4; x < 0; x++)
            {
                GameObject wPrefab = Instantiate(wallPrefab[rnd.Next(0, wallPrefab.Length - 1)], new Vector3(x, y), Quaternion.identity) as GameObject;
                wPrefab.transform.parent = Enviroment.transform;
                wallDictionary.Add(wPrefab.transform.position, wPrefab);
            }
            for (int x = width; x < (width + 4); x++)
            {
                GameObject wPrefab = Instantiate(wallPrefab[rnd.Next(0, wallPrefab.Length - 1)], new Vector3(x, y), Quaternion.identity) as GameObject;
                wPrefab.transform.parent = Enviroment.transform;
                wallDictionary.Add(wPrefab.transform.position, wPrefab);
            }
        }


        if (player == null) player = new Player(mapa.entrance);                                                 // Spawni hráče, pokud neexistuje
        playerGameObject = Instantiate(playerPrefab, mapa.entrance, Quaternion.identity) as GameObject;         // Instancuj hráčův GameObject
        playerGameObject.GetComponent<PlayerScript>().HumanPlayer = player;                                     // Přiřaď reference do hráčova skriptu
        player.position = mapa.entrance;
        playerGameObject.GetComponent<PlayerScript>().clock = tickGenerator;
        playerGameObject.name = "Player";                                                                       // Přejmenuj hráče na "Player"
        playerGameObject.GetComponent<PlayerScript>().HumanPlayer.PutWeaponInHand(player.equippedWeapon, playerGameObject.transform);
        playerGameObject.transform.position = mapa.entrance;                                                    // Přesuň hráčův GO na vchod
        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, -10);
        Camera.main.GetComponent<CameraHandler>().target = playerGameObject.transform;

    }

    public void DestroyScene()
    {
        Debug.Log("GameMap: Destroying the scene.");
        List<GameObject> GameObjectsToDestroy = new List<GameObject>();

        GameObjectsToDestroy.AddRange(objectDictionary.Values);
        GameObjectsToDestroy.AddRange(enemyList);
        GameObjectsToDestroy.AddRange(wallDictionary.Values);
        GameObjectsToDestroy.AddRange(corridorGroundDictionary.Values);
        GameObjectsToDestroy.AddRange(dungeonGroundDictionary.Values);
        GameObjectsToDestroy.AddRange(GameObject.FindGameObjectsWithTag("GameHUD"));
        player = GameObject.Find("Player").GetComponent<PlayerScript>().HumanPlayer;
        player.equippedWeapon = GameObject.Find("Player").GetComponent<PlayerScript>().HumanPlayer.equippedWeapon;
        GameObjectsToDestroy.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        foreach (GameObject g in GameObjectsToDestroy)
        {
            Destroy(g);
        }
        
        objectDictionary.Clear();
        enemyList.Clear();
        wallDictionary.Clear();
        dungeonGroundDictionary.Clear();
        corridorGroundDictionary.Clear();
    }

    void SetPlayerName()    // Tato funkce bere hodnotu z předchozí scény a nastavuje hráčovi jméno
    {
        if (GameObject.Find("PlayerNameHandler") != null)
        {
            playerGameObject.GetComponent<PlayerScript>().HumanPlayer.name = GameObject.Find("PlayerNameHandler").GetComponent<PlayerNameHandler>().playerName;
            Destroy(GameObject.Find("PlayerNameHandler"));
            Debug.Log("Player name: " + playerGameObject.GetComponent<PlayerScript>().HumanPlayer.name);
        }
    }

    public void CreateEnemies()
    {
        int numOfEnemiesInRound_low = Mathf.CeilToInt((float)0.58333333333333 * (float)level + (float)2.4166666666667);     // Prostě dvě lineární funkce
        int numOfEnemiesInRound_high = Mathf.CeilToInt((float)1.3333333333333*(float)level + (float)2.6666666666667);      

        int numOfEnemies = rndNum.Next(numOfEnemiesInRound_low, numOfEnemiesInRound_high+1);
        Debug.Log("GameMap: Creating " + numOfEnemies + " enemies.");


        float k = ((float)weaponPrefabsSorted.Length - 1) / ((float)endLevel - 2);       // Zjisti, kterou zbraň equipnout na základě lvl
        float q = (1 - k);
        int weaponNumToInstantiate = Mathf.CeilToInt(k * level + q);

                        //Enemy enemyToAssign = new Enemy(new Vector2(player.position.x + 3, player.position.y), 5, 3, 10);
        //enemyToAssign.restrictedToDiagonalMovementOnly = true;
        //GameObject testrEnemy = Instantiate(testEnemy, new Vector2(player.position.x + 3, player.position.y), Quaternion.identity) as GameObject;
        //testrEnemy.GetComponent<EnemyScript>().enemy = enemyToAssign;

        
        //GameObject weapon = Instantiate(weaponPrefabsSorted[rndNum.Next(0, weaponNumToInstantiate)], Vector2.zero, Quaternion.identity) as GameObject;

        //testrEnemy.GetComponent<EnemyScript>().enemy.PutWeaponInHand(weapon, testrEnemy.transform);

        //enemyList.Add(testrEnemy);

        //Vector2 dposToInstantiate = getRandomPosForEnemy();
        //GameObject zombieGdO = Instantiate(zombiePrefab, dposToInstantiate, Quaternion.identity) as GameObject;
        //zombieGdO.GetComponent<EnemyScript>().weapToEquip = weaponPrefabsSorted[rndNum.Next(0, weaponNumToInstantiate + 1)];

        int spawnedEnemies = 0;
        while (enemyList.Count <= numOfEnemies)
        {
            Vector2 posToInstantiate;

            if (d(Mathf.CeilToInt(100 / Mathf.CeilToInt((float)level / (float)2))))     // Spawn Blob
            {
                posToInstantiate = getRandomPosForEnemy();

                GameObject blobGO = (GameObject)Instantiate(blobPrefab, posToInstantiate, Quaternion.identity);

                enemyList.Add(blobGO);
                spawnedEnemies++;
            }
            else if (d(200))                                                            // Spawn Zombie
            {
                posToInstantiate = getRandomPosForEnemy();

                GameObject zombieGO = (GameObject)Instantiate(zombiePrefab, posToInstantiate, Quaternion.identity);

                if (d(700))
                {
                    zombieGO.GetComponent<EnemyScript>().weapToEquip = weaponPrefabsSorted[rndNum.Next(0, weaponNumToInstantiate)];
                }

                enemyList.Add(zombieGO);
                spawnedEnemies++;
            }
            else if (d(Mathf.CeilToInt((100 * (float)level) / (float)endLevel)))         // Spawn Skeleton
            {
                posToInstantiate = getRandomPosForEnemy();

                GameObject skeletonGO = (GameObject)Instantiate(skeletonPrefab, posToInstantiate, Quaternion.identity);

                if (d(500))
                {
                    skeletonGO.GetComponent<EnemyScript>().weapToEquip = weaponPrefabsSorted[rndNum.Next(0, weaponNumToInstantiate)];
                }

                enemyList.Add(skeletonGO);
                spawnedEnemies++;
            }
        }
    }

    Vector2 getRandomPosForEnemy()
    {
        List<Vector2> keyList = new List<Vector2>(dungeonGroundDictionary.Keys);

        int counter = 0;
        while (counter < keyList.Count * 100)
        {
            counter++;
            Vector2 tempPos = keyList[rndNum.Next(0, keyList.Count)];
            bool enemyAtTempPos = false;
            foreach (GameObject enemyGo in enemyList)
            {
                if (tempPos == new Vector2(enemyGo.transform.position.x, enemyGo.transform.position.y))
                {
                    enemyAtTempPos = true;
                }
            }

            if (!enemyAtTempPos && tempPos != playerGameObject.GetComponent<PlayerScript>().HumanPlayer.position)
            {
                return tempPos;
            }
        }

        Debug.Log("Error finding pos for enemy to spawn. Returning Vector2(-1, -1).");
        return new Vector2(-1, -1);
    }

    public void generateDrops()
    {
        System.Random rnd = new System.Random(System.DateTime.Now.GetHashCode());
        
        foreach (Vector2 pos in dungeonGroundDictionary.Keys)
        {
            if (!objectDictionary.ContainsKey(pos) && !(pos == mapa.entrance))     // Pokud objectDictionary neobsahuje klíč na tomto místě a není to spawnpoint
            {

                if (d(13))
                {
                    GameObject foodPickup = Instantiate(foodPrefab[rnd.Next(0, foodPrefab.Length)], pos, Quaternion.identity) as GameObject;
                    objectDictionary.Add(pos, foodPickup);
                }
                else if (d(4))
                {
                    GameObject healthPack = Instantiate(healthPackFrefab[rnd.Next(0, healthPackFrefab.Length)], pos, Quaternion.identity) as GameObject;
                    objectDictionary.Add(pos, healthPack);
                }
                else if ((d(3)))
                {
                    GameObject armorPickup = Instantiate(armorPrefab[rnd.Next(0, armorPrefab.Length)], pos, Quaternion.identity) as GameObject;
                    objectDictionary.Add(pos, armorPickup);
                }
                else if (d(3))
                {
                    float k = ((float)weaponPrefabsSorted.Length - 1) / ((float)endLevel - 2);
                    float q = (1 - k);
                    int weaponNumToInstantiate = Mathf.CeilToInt(k*level + q);
                    GameObject weapon = Instantiate(weaponPrefabsSorted[rndNum.Next(0, weaponNumToInstantiate)], pos, Quaternion.identity) as GameObject;
                    objectDictionary.Add(pos, weapon);
                }
            }
        }
    }

    System.Random rndNum = new System.Random();
    bool d(int chanceInThousand)
    {
        if (rndNum.Next(0, 1000) < chanceInThousand)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}









