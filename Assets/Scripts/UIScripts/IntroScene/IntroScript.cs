using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour {

    public Image[] image;
    public float fadeTime = 1F;
    public float imageViewTime = 3F;

    // 0 = průhledné; 255 = viditelné

	void Start () {

        foreach (Image img in image) img.color = new Color32(255, 255, 255, 1);   // Všechny obrázky nejprve zneviditelním
                                                                                  // Na všech obrázcích musí být color a != 0. Proč? Bug?

        StartCoroutine(Fade());
	}

    IEnumerator Fade()
    {
        for (int i = 0; i < image.Length; i++)
        {
            image[i].CrossFadeAlpha(255, fadeTime, false);          // Fade the image in
            yield return new WaitForSeconds(imageViewTime);         // Wait n seconds
            image[i].CrossFadeAlpha(0, fadeTime, false);            // Fade the image out
        }
        SceneManager.LoadScene("StartMenu");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("StartMenu");
        }
    }
	
}
