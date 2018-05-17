using UnityEngine;
using System.Collections;

public class TickGenerator : MonoBehaviour {

    public float timer = 0;
    public float interval = 2;
    public long round = 0;
    public bool enabled = false;

    GameMap GameMap;
    public PlayerScript PlayerScript;

    void Start()
    {
        GameMap = GameObject.Find("GameMap").GetComponent<GameMap>();
        PlayerScript = GameMap.playerGameObject.GetComponent<PlayerScript>();
    }

	void FixedUpdate () {
        if (enabled)
        {
            timer += Time.deltaTime;
            if (timer > interval)
            {
                NextRound();
            }
        }
	}

    public void NextRound()
    {
        if (enabled)
        {
            CycleThroughEnemiesAndPlayer();
            timer = 0;
            round++;
        }
    }

    // Tahle funkce se zavolá u každého enemy z GameMap a spustí se tak u něj funkce OneRound();
    void CycleThroughEnemiesAndPlayer()
    {
        PlayerScript.OneRound();
        foreach (GameObject e in GameMap.enemyList)
        {
            e.GetComponent<EnemyScript>().OneRound();
        }
    }
}

