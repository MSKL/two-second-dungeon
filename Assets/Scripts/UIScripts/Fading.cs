using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {

	public Texture2D fadeOutTexture;	// textura která překryje obrazovku
	public float fadeSpeed = 0.8f;		// rychlost fade

	private float alpha = 1.0f;			// alpha hodnota mezi 0 a 1
	private int fadeDir = -1;			// směr fadě -> -1 = in / 1 = out

	void OnGUI()
	{
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);


		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = -1000;																    // Textura se musí vykreslit navrchu
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

    public float BeginFade(int direction)    // směr fadu -> in = -1, out = 1
	{
		fadeDir = direction;
		return (fadeSpeed);
	}

	void OnLevelWasLoaded()
	{
		// alpha = 1;
		BeginFade(-1);		// Zavolej funkci po načtení levelu
	}
}

