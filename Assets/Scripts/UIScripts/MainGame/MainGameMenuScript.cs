using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainGameMenuScript : MonoBehaviour {

    public GameObject MainGameMenuPanel;
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MainGameMenuPanel.activeSelf == false)
            {
                MainGameMenuPanel.SetActive(true);
                GameMap.Instance.tickGenerator.enabled = false;
            }
            else if (MainGameMenuPanel.activeSelf == true)
            {
                MainGameMenuPanel.SetActive(false);
                GameMap.Instance.tickGenerator.enabled = true;
            }
        }
	}

    public void ExitToMainMenu()
    { 
        SceneManager.LoadScene("StartMenu");
        Destroy(GameObject.Find("GameMap"));
    }
}
