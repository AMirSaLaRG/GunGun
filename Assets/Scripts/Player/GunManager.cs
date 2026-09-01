using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Analytics.IAnalytic;

public class GunManager : MonoBehaviour
{
    private TouchControls controls;


    private WeaponData currentGunData;
    private MagazineManager magazineManager;
    private HitDitector hitDitector;
    private PlayerController player;
    private ShopManager shopManager;


    [Header("All Guns")]
    public List<GunSo> allGuns = new List<GunSo>();  // All guns in the game
    public Dictionary<int, GunSo> gunDictionary { private set; get; } = new Dictionary<int, GunSo>();

    [Header("Player's Unlocked Guns")]
    public List<int> unlockedGunsId = new List<int>();

    [Header("Current Gun")]
    public GunSo currentGun { private set; get; }
    public int currentGunId { private set; get; } = 0;

    [Header("Settings")]
    [SerializeField] private Transform magazineSocket;
    [SerializeField] private GameObject defaultHitVfx;
    [SerializeField] private GameObject defaultbulletHole;
    [SerializeField] private float holeLifetime;
    [SerializeField] private float changeGunTime = 0.3f;

    [Header ("Sfx")]
    [SerializeField] private AudioSource defaultHitSfx;
    [SerializeField] private AudioSource gunShotSfx;
    [SerializeField] private AudioSource gunEmptyShotSfx;

    private async void Start()
    {
        await LoadAllGuns();

        foreach (var gun in allGuns)
            gunDictionary[gun.gunID] = gun;

        player = GetComponent<PlayerController>();
        magazineManager = GetComponent<MagazineManager>();
        hitDitector = GetComponent<HitDitector>();
        shopManager = FindFirstObjectByType<ShopManager>();

        

        GetUnlockedGunsId();

        currentGunId = GameManager.instance.playerData.lastGunId;

        ResetTheGun();
    }

    async System.Threading.Tasks.Task LoadAllGuns()
    {
        // Load all assets with label "Gun" (you set this earlier)
        var handle = Addressables.LoadAssetsAsync<GunSo>("Gun", null);

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            allGuns = new List<GunSo>(handle.Result);
            Debug.Log($"Loaded {allGuns.Count} guns!");
        }
        else
        {
            Debug.LogError("Failed to load guns!");
        }
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
            shopManager.GunPurchase(gunDictionary[0]);
            unlockedGunsId.AddRange(GameManager.instance.playerData.unlockedWeapons);
        }
    }

    public bool IsGunUnlocked(int gunId, out GunSo gunData)
    {
        bool isUnlocked = unlockedGunsId.Contains(gunId);
        gunData = allGuns.Find(x => x.gunID ==  gunId);

        return isUnlocked;
    }
    public List<int> GetUnlockedGunsId()
    {
        RefreshUnlockGuns();

        return unlockedGunsId;

    }

    private void SwitchGunOne(InputAction.CallbackContext context)
    {
        Debug.Log("Triggered Test");

        currentGunId = (currentGunId + 1) % unlockedGunsId.Count;
        SetCurrentGun(currentGunId);
    }

    public void SetCurrentGun(int gunId)
    {
        GunSo gun;

        if (gunDictionary.ContainsKey(gunId))
        {
            gun = gunDictionary[gunId];
        } else
        {
            gun = gunDictionary[0];
        }
        currentGun = gun;
        currentGunData = gun.WeaponData;

        currentGunId = gunId;

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
        magazineManager.PutMagazineInSocket(changeGunTime);
    }

    public List<HitData> GunShot(Vector2 shotAim)
    {
        if (magazineManager.isReloading)
            return null;

        List<HitData> hitDatas = GatherHitData(shotAim);

        if (hitDatas == null) return null;
        if (hitDatas.Count == 0) return null;

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

            case GunShotType.ThripleShot:
                resault.AddRange(hitDitector.MultiShot(shotAim, currentGunData.accuracy, 3));
                break;


            default:
                resault = null;
                break;
        }

        return resault;
    }

    private bool OnShotHit(List<HitData> hitDatas)
    {
        if (hitDatas == null) return false;
        if (hitDatas.Count == 0) return false;
        foreach (var hit in hitDatas)
        {
            if (hit == null)
            {
                PlayGunShotSfx();
                StartCoroutine(OnFreeSHotAirCo());
                return false;
            }

        }


        HitData magaizne = hitDatas.Find(x => x.hitTYpe == EHitType.Magazine);
        if (magaizne != null)
        {
            magazineManager.OnReloadBullets();
            return false;
        }

        if (magazineManager.IsEmpty())
        {
            PlayEmptyShotSfx();
            return false;
        }
        else
            PlayGunShotSfx();

        StartCoroutine(GunFireCo(hitDatas));

        return true;
    }

    private IEnumerator OnFreeSHotAirCo()
    {
        bool isShootingBullet = magazineManager.ConsumeAmmo(1);
        yield return new WaitForSeconds(currentGunData.rapitShootingDelay);

    }

    private IEnumerator GunFireCo(List<HitData> hitDatas)
    {
        Target hitedTarget;
        Vector3 hitedPos;

        foreach (HitData hitData in hitDatas)
        {
            bool isShootingBullet = magazineManager.ConsumeAmmo(1);
            if (isShootingBullet)
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
                    hitData.hitTYpe = hitDitector.GetTargetStatus(hitData.hitedTarget);
                    hitedTarget.TakeDamage(currentGunData.gunDamage, hitData);
                }

                player.HandleShot(hitData);
            }
            yield return new WaitForSeconds(currentGunData.rapitShootingDelay);
        }      
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

    public void ResetTheGun()
    {
        SetCurrentGun(0);
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
