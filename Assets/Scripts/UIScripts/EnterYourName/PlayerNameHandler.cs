using UnityEngine;
using System.Collections;

public class PlayerNameHandler : MonoBehaviour {

    public string playerName;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
