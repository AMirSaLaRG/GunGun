using System;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    [Header("Basic Info")]
    public int gunID;
    public string gunName;
    public GunShotType gunShotType;

    [Header("Stats")]
    public int gunDamage = 10;
    public int bulletsOnEachShot = 1;
    public float gunPierce = 0f;
    public float gunPrice = 100f;
    public float gunForce = 500f;
    public float gunReloadTime = 2f;
    public int magazineAmoCap = 2;
    public float rapitShootingDelay = 0;
    [Range (.2f, 1)]public float accuracy = 1;

    [Header("Multipliers")]
    public float pointMultiplier = 1f;
    public float comboMultiplier = 1f;

    [Header("Audio (Set in scene)")]
    public AudioClip gunShotClip;
    public AudioClip gunEmptyClip;
    public AudioClip defaultHitClip;

    [Header("Visual Effects")]
    public GameObject defaultHitVfx;
    public GameObject bulletHolePrefab;


}

public enum GunShotType 
{
    singleShot = 0,
    ThripleShot = 1,
    machineGun = 2,
    shotgun = 3,
}