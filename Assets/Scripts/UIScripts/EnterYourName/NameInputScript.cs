using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NameInputScript : MonoBehaviour {

    InputField vstupniPole;
    PlayerNameHandler playerNameHandler;
    float time;

    void Start(){
        vstupniPole = GetComponent<InputField>();

        EventSystem.current.SetSelectedGameObject(gameObject, null);                // Tyto dvě řádky zajišťují, že je ukazatel už od začátku v poli
        vstupniPole.OnPointerClick(new PointerEventData(EventSystem.current));
        playerNameHandler = GameObject.Find("PlayerNameHandler").GetComponent<PlayerNameHandler>();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time > 1)                                                               // Ochrana proti nechtěnému překliknutí
        {
            if (Input.GetKeyDown("return") || Input.GetKeyDown(KeyCode.Return))     // reaguj na stisk Enter nebo Return
            {
                print("Hracovo jmeno: " + vstupniPole.textComponent.text);
                playerNameHandler.playerName = vstupniPole.textComponent.text;            // ulož hráčovo jméno do PlayerNameHandleru
                StartCoroutine("ChangeLevel");
            }
        }
    }

    IEnumerator ChangeLevel()
    {
        gameObject.GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("MainGame");
    }

    void TextChanged()
    {
        time = 0;
    }

}
