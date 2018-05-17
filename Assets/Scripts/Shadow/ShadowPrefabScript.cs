using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class ShadowPrefabScript : MonoBehaviour {

    SpriteRenderer sr;
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public void FadeIn(float time)
    {
        StartCoroutine(FadeOut(0, 1, time));
    }

    public void FadeOut(float time)
    {
        StartCoroutine(FadeOut(1, 0, time));
    }

    private IEnumerator FadeOut(float alphaStart, float alphaFinish, float time)
    {
        float elapsedTime = 0;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alphaStart);

        while (elapsedTime < time)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Lerp(sr.color.a, alphaFinish, (elapsedTime / time)));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
