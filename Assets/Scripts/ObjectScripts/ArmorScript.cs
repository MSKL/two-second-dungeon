using UnityEngine;
using System.Collections;

public class ArmorScript : MonoBehaviour {

    public float armor;

    [Range(1, 100)]
    public int lowerBound;

    [Range(1, 100)]
    public int higherBound;
    void Start()
    {
        System.Random rnd = new System.Random();
        armor = rnd.Next((int)lowerBound, (int)higherBound + 1);
    }
}
