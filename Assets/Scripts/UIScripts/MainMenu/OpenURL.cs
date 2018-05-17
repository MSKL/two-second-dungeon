using UnityEngine;
using System.Collections;

public class OpenURL : MonoBehaviour {

    public void OpenTheURL()
    {
        Application.OpenURL("http://dungeon2.stoked.cz/");
    }
}
