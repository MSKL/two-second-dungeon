using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LastSceneScript : MonoBehaviour {

    public Image lastImage;
    float startTime;

	void Start () {
        startTime = Time.time;
	}

    void Update()
    {

        if (Time.time - startTime > 4)
        {
            lastImage.color = new Color32(255, 255, 255, 1);
            lastImage.CrossFadeAlpha(255, 1F, false);  
        }

        if((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Y)) && (Time.time - startTime > 5)) {
            SceneManager.LoadScene("StartMenu");
        }
    }
}
