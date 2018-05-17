using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuHighscoreList : MonoBehaviour {

    public bool visible = false;
    public Text leaderboardText;

    void Start()
    {
        gameObject.SetActive(visible);
    }

    public void ChangeState()
    {
        visible = !visible;
        if (visible == true)
        {
            DrawHighScores();
        }
        gameObject.SetActive(visible);

    }

    public void DrawHighScores()
    {

        int counter = 0;        // Counter jsou jen sudá čísla
                                // sudý = skóre
                                // lichý (sudý + 1) = jméno

        System.Collections.Generic.SortedList<float, string> sortedHighscoreList = new System.Collections.Generic.SortedList<float, string>();

        while (PlayerPrefs.HasKey(counter.ToString()))                  // Načtu všechny hodnoty z PlayerPrefs do SortedList<float = score, string = name>
        {
            string playerName = PlayerPrefs.GetString((counter+1).ToString());
            float playerScore = PlayerPrefs.GetFloat(counter.ToString());
            while (sortedHighscoreList.ContainsKey(playerScore))
            {
                playerScore++;                                          // Pokud by již skóre existovalo, pro jednoduchost mu přičtu+1 abych nemusel řešit složitě sort
            }
            sortedHighscoreList.Add(playerScore, playerName);
            counter += 2;
        }

        leaderboardText.text = "";

        for (int i = sortedHighscoreList.Count - 1; i >= 0; i--)
        {
            string playerName = sortedHighscoreList.Values[i];
            float playerScore = sortedHighscoreList.Keys[i];
            leaderboardText.text += sortedHighscoreList.Count - i + ". " + playerName + ": " + playerScore + "\r\n";
        }

    }

    void Update()
    {
        if (gameObject.activeSelf)  // Pokud je GameObject aktivní
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ChangeState();
            }
        }
    }

}
