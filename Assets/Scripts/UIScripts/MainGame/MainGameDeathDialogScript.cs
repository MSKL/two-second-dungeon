using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainGameDeathDialogScript : MonoBehaviour {


    public GameObject DeathPanelGameObject;
    public Text scoreText;

    public void showDeathPanel(float score)
    {
        DeathPanelGameObject.SetActive(true);
        scoreText.text = "Přežil jsi " + score + " kol.";
    }
}
