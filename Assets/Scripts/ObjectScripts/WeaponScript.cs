using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

    [Header("Damage between <min, max>: ")]

    [Range(0, 100)]
    public int minDamage;

    [Range(0, 100)]
    public int maxDamage;

    [HideInInspector()]
    public int damage;

    public string name = "Unknown";

    void Start()
    {
        damage = Random.Range(minDamage, maxDamage + 1);
    }

    public void Destroy()
    {
        if (GameMap.Instance.objectDictionary.ContainsKey(transform.position))
        {
            GameMap.Instance.objectDictionary.Remove(transform.position);
        }
        Destroy(gameObject);
    } 

}
