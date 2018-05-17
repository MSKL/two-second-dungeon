using UnityEngine;
using System.Collections;

public class FoodScript : MonoBehaviour {

    public float nutrition;

    [Range(1, 100)]
    public int lowerBound;

    [Range(1, 100)]
    public int higherBound;
	void Start () {
        System.Random rnd = new System.Random();
        nutrition = rnd.Next((int)lowerBound, (int)higherBound + 1);
	}

}
