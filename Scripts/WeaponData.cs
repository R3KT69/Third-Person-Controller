using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponImage;
    public int damage;
    public int ammo;
    public float fireRate;
    public float range;
}
