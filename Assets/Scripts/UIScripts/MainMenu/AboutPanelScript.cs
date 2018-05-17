using UnityEngine;
using System.Collections;

public class AboutPanelScript : MonoBehaviour {

    public bool visible = false;

    void Start()
    {
        gameObject.SetActive(visible);
    }

    public void ChangeState()
    {
        visible = !visible;
        gameObject.SetActive(visible);
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
