using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="NewGun", menuName = "Game/ Create Gun")]
public class GunSo : ScriptableObject
{


    [Header("Gun Preview")]
    public Sprite gunIcon;
    [TextArea] public string aboutGun;

    [Header("Weapon Data")]
    public WeaponData WeaponData;

    [Header("Magazine")]
    public GameObject gunMagazinePrefab;

    [Header("Identification")]
    [Tooltip("Must be unique! Use 1, 2, 3, 4, 5...")]
    public int gunID;

}
