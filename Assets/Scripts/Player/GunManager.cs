using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : MonoBehaviour
{
    private TouchControls controls;


    private WeaponData currentGunData;
    private MagazineManager magazineManager;
    private HitDitector hitDitector;

    [Header("All Guns")]
    public List<GunSo> allGuns = new List<GunSo>();  // All guns in the game
    public Dictionary<int, GunSo> gunDictionary { private set; get; } = new Dictionary<int, GunSo>();

    [Header("Player's Unlocked Guns")]
    public List<int> unlockedGunsId = new List<int>();

    [Header("Current Gun")]
    public GunSo currentGun;
    public int currentGunIndex = 0;

    [Header("Settings")]
    [SerializeField] private Transform magazineSocket;
    [SerializeField] private GameObject defaultHitVfx;
    [SerializeField] private GameObject defaultbulletHole;
    [SerializeField] private float holeLifetime;

    [Header ("Sfx")]
    [SerializeField] private AudioSource defaultHitSfx;
    [SerializeField] private AudioSource gunShotSfx;
    [SerializeField] private AudioSource gunEmptyShotSfx;

    private void Start()
    {
        magazineManager = GetComponent<MagazineManager>();
        hitDitector = GetComponent<HitDitector>();

        foreach (var gun in allGuns)
            gunDictionary[gun.gunID] = gun;

        GetUnlockedGunsId();

        currentGunIndex = 0;

        SetCurrentGun();
    }

    private void RefreshUnlockGuns()
    {
        unlockedGunsId.Clear();

        if (GameManager.instance == null)
            return;
        
        if (GameManager.instance.playerData.unlockedWeapons != null)
            unlockedGunsId.AddRange(GameManager.instance.playerData.unlockedWeapons);

        if (unlockedGunsId.Count == 0)
        {
            GameManager.instance.GunPurche(gunDictionary[0]);
            unlockedGunsId.AddRange(GameManager.instance.playerData.unlockedWeapons);
        }
    }

    public List<int> GetUnlockedGunsId()
    {
        RefreshUnlockGuns();

        return unlockedGunsId;

    }

    private void SwitchGunOne(InputAction.CallbackContext context)
    {
        Debug.Log("Triggered Test");

        currentGunIndex = (currentGunIndex + 1) % unlockedGunsId.Count;
        SetCurrentGun(currentGunIndex);
    }

    public void SetCurrentGun(int gunIndex = 0)
    {
        GunSo gun = gunDictionary[0];

        int gunId = unlockedGunsId[gunIndex];
        if (gunDictionary.ContainsKey(gunId))
        {
            gun = gunDictionary[gunId];
        } else
        {
            Debug.Log("Not valid Id");
        }
        currentGun = gun;
        currentGunData = gun.WeaponData;

        SetupMagazine(gun);

    }

    private void SetupMagazine(GunSo gun)
    {
        while (magazineSocket.childCount > 0)
        {
            DestroyImmediate(magazineSocket.GetChild(0).gameObject);
        }

        GameObject newMagazine = Instantiate(gun.gunMagazinePrefab, magazineSocket);
        magazineManager = newMagazine.GetComponent<MagazineManager>();
        magazineManager.PutMagazineInSocket();
    }

    public List<HitData> GunShot(Vector2 shotAim)
    {
        if (magazineManager.isReloading)
            return null;

        List<HitData> hitDatas = GatherHitData(shotAim);

        if (OnShotHit(hitDatas))
            return hitDatas;

        return null;
    }



    private List<HitData> GatherHitData(Vector2 shotAim)
    {
        List<HitData> resault = new List<HitData>();

        switch (currentGunData.gunShotType)
        {
            case GunShotType.singleShot:
                resault.Add(hitDitector.SingleShot(shotAim));
                break;

            default:
                return null;
        }

        return resault;
    }

    private bool OnShotHit(List<HitData> hitDatas)
    {
        //foreach (var hit in hitDatas)
        //    Debug.Log(hit.hitTYpe);

        if (hitDatas == null) return false;

        HitData magaizne = hitDatas.Find(x => x.hitTYpe == EHitType.Magazine);
        if (magaizne != null)
        {
            magazineManager.OnReloadBullets();
            return false;
        }

        Target hitedTarget;
        Vector3 hitedPos;

        bool isShootingBullet = magazineManager.ConsumeAmmo(1);

        if (isShootingBullet == false)
        {
            PlayEmptyShotSfx();
            return false;
        }
        else
            PlayGunShotSfx();
        


 

        foreach (HitData hitData in hitDatas)
        {
            hitData.hitForce = currentGunData.gunForce;
            hitedTarget = hitData.hitedTarget ?? null;
            hitedPos = hitData.hitPos;

            if (hitedTarget == null)
                if (hitData.hitPos != null && hitData.hitTYpe != EHitType.Magazine)
                {
                    if (currentGunData.defaultHitClip != null)
                        defaultHitSfx.clip = currentGunData.defaultHitClip;
                    if (currentGunData.defaultHitVfx != null)
                        defaultHitVfx = currentGunData.defaultHitVfx;
                    if (currentGunData.bulletHolePrefab != null)
                        defaultbulletHole = currentGunData.bulletHolePrefab;

                    DefaultSfxVfxHandler(hitData);        
                }

            if (hitedTarget != null)
            {           
                hitedTarget.TakeDamage(currentGunData.gunDamage, hitData); 
            }
          
        }

        return true;
    }

    private void DefaultSfxVfxHandler(HitData hitData)
    {
        AudioManager.instance.PlaySfx(defaultHitSfx, true);

        Quaternion rotation = Quaternion.LookRotation(hitData.hitNorm);

        Instantiate(defaultHitVfx, hitData.hitPos, rotation);

        MakeHoleOnHit(hitData);
    }

    private void MakeHoleOnHit(HitData hit)
    {
        Vector3 placePos = hit.hitPos + (hit.hitNorm * 0.002f);
        Quaternion rotation = Quaternion.LookRotation(hit.hitNorm);

        GameObject hole = Instantiate(defaultbulletHole, placePos, rotation);
        hole.transform.parent = hit.hitTransform;

        Destroy(hole, holeLifetime);
    }

    private void PlayGunShotSfx()
    {
        if (currentGunData.gunShotClip != null)
            gunShotSfx.clip = currentGunData.gunShotClip;

        AudioManager.instance.PlaySfx(gunShotSfx, true);
    }

    private void PlayEmptyShotSfx()
    {
        if (currentGunData.gunEmptyClip != null)
            gunEmptyShotSfx.clip = currentGunData.gunEmptyClip;

        AudioManager.instance.PlaySfx(gunEmptyShotSfx, true);
    }

    public void Reload()
    {
        if (magazineSocket != null)
            magazineManager.OnReloadBullets();
    }

    public void ReadyTheGun()
    {
        SetCurrentGun(currentGunIndex);
    }
    private void OnEnable()
    {
        // Initialize and setup controls here
        if (controls == null)
            controls = new TouchControls();

        controls.TouchScreen.Test.performed += SwitchGunOne;
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.TouchScreen.Test.performed -= SwitchGunOne;
        controls.Disable();
    }
}
