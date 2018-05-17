using UnityEngine;
using System.Collections;

public class Weapon {

    public WeaponType weaponType;
    public float damage;
    public string name;

    public enum WeaponType
    {
        WoodenBranch,
        SteelStick,
        LongSword,
        WaterSword,
        KingSlayer
    }

    public Weapon(WeaponType _weaponType)
    {
        weaponType = _weaponType;

        System.Random rnd = new System.Random();


        switch (_weaponType)
        {
            case WeaponType.WoodenBranch:        // Pokud se jedná o WoodenBranch, je damage mezi 1 a 5
                damage = rnd.Next(1, 6);
                name = "Wooden Branch";
                break;
            case WeaponType.SteelStick:
                damage = rnd.Next(3, 10);
                name = "Steel Stick";
                break;
            case WeaponType.LongSword:
                damage = rnd.Next(9, 18);
                name = "Long Sword";
                break;
            case WeaponType.WaterSword:
                damage = rnd.Next(12, 20);
                name = "WaterSword";
                break;
            case WeaponType.KingSlayer:
                damage = rnd.Next(20, 50);
                name = "King Slayer";
                break;
            default:
                Debug.Log("Unknown Weapon Enum");
                name = "Unknown";
                damage = 1;
                break;
        }
    }
}
