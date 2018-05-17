using UnityEngine;
using System.Collections;

public class HealthPackScript : MonoBehaviour {

    public float health;

    [Range(1, 100)]
    public int lowerBound;

    [Range(1, 100)]
    public int higherBound;
    void Start()
    {
        System.Random rnd = new System.Random();
        health = rnd.Next((int)lowerBound, (int)higherBound + 1);
    }
}
