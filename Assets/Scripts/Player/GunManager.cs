using DG.Tweening;
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
    public Dictionary<int, MagazineManager> magazineDisctionary { private set; get; } = new Dictionary<int, MagazineManager>();

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
    [SerializeField] private float changeGunTime = 0.05f;

    [Header ("bullet trail")]
    [SerializeField] private GameObject bulletLinePrefab;
    [SerializeField] private Transform bulletLinesHolder;
    [SerializeField] private float bulletLineShowTime = .2f;

    public Vector3 StartingPos0bulletTrail { get; private set; }


    private int reserveBulletLineNum = 20;
    private int currentBulletLineIndex = 0;
    private List<LineRenderer> bulletLinesList;

    

    [Header ("Sfx")]
    [SerializeField] private AudioSource defaultHitSfx;
    [SerializeField] private AudioSource gunShotSfx;
    [SerializeField] private AudioSource gunEmptyShotSfx;

    private bool isShooting = false;

    private int mainWeaponId;
    private int secondaryWeaponId;
    public bool hasSecondaryWeapon { private set; get; } = false;

    private float reloadTimeScale = 1;

    private Coroutine fireCo;
    private Coroutine bulletLineCo;

    private async void Start()
    {
        await LoadAllGuns();

        foreach (var gun in allGuns)
            gunDictionary[gun.gunID] = gun;

        SetUpMagazine();



        player = GetComponent<PlayerController>();
        hitDitector = GetComponent<HitDitector>();
        shopManager = FindFirstObjectByType<ShopManager>();

        

        GetUnlockedGunsId();

        currentGunId = GameManager.instance.playerData.lastGunId;

        ResetTheGun();
        CreateReserveBulletLines();
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

    public bool IsGunUnlocked(int gunId)
    {
        bool isUnlocked = unlockedGunsId.Contains(gunId);

        return isUnlocked;
    }

    public GunSo GetGunData(int gunId)
    {
        if (gunDictionary.ContainsKey(gunId))
            return gunDictionary[gunId];
        return null;
    }
    public List<int> GetUnlockedGunsId()
    {
        RefreshUnlockGuns();

        return unlockedGunsId;

    }

    private void SetUpMagazine()
    {

        while (magazineSocket.childCount > 0)
        {
            magazineSocket.GetChild(0).gameObject.transform.DOKill();

            DestroyImmediate(magazineSocket.GetChild(0).gameObject);
        }

        foreach (var gun in allGuns)
        {
            if (gun.gunMagazinePrefab != null)
            {
                MagazineManager newMagazineManager;
                GameObject newMagazine = Instantiate(gun.gunMagazinePrefab, magazineSocket);
                newMagazineManager = newMagazine.GetComponent<MagazineManager>();
                magazineDisctionary[gun.gunID] = newMagazineManager;
                newMagazineManager.SetManager(this);
                newMagazineManager.gameObject.SetActive(false);
            }
        }
    }

    public void ChangeReloadTimeScale(float timeScale) => reloadTimeScale = timeScale;
    
    public void SetCurrentGun(int gunId, bool isInstant = false, bool shouldReload = true, bool useBulletsFromAnotherMagazine = false)
    {
        StartCoroutine(SetCurrentGunCo(gunId, isInstant, shouldReload, useBulletsFromAnotherMagazine));
    }

    private IEnumerator SetCurrentGunCo(int gunId, bool isInstant = false, bool shouldReload = true, bool useBulletsFromAnotherMagazine = false)
    {
        int saftylimit = 250;
        while (isShooting || saftylimit <= 0)
        {   
            yield return null;
            yield return new WaitForSeconds(currentGunData.rapitShootingDelay);
            saftylimit--;
            if (saftylimit <= 1)
                Debug.Log("LimitBreak");
        }

        isShooting = false;
        GunSo gun;
        if (gunDictionary.ContainsKey(gunId))
        {
            gun = gunDictionary[gunId];
        }
        else
        {
            gun = gunDictionary[0];
        }
        currentGun = gun;
        currentGunData = gun.WeaponData;

        currentGunId = gunId;

        int currentAmo;

        if (shouldReload)
            if (useBulletsFromAnotherMagazine)
                currentAmo = magazineManager.currentAmo;
            else
                currentAmo = -1;
        else
            currentAmo = -1;

        ActivateMagazine(gun, isInstant, shouldReload, currentAmo);
    }

    private void ActivateMagazine(GunSo gun, bool isInstant,bool shouldReload, int currentAmo)
    {

        foreach(var mag in magazineDisctionary)
            mag.Value.gameObject.SetActive(false);

        magazineManager = magazineDisctionary[gun.gunID];
        magazineManager.gameObject.SetActive(true);

        magazineManager.PutMagazineInSocket(changeGunTime, isInstant, shouldReload, currentAmo);
    }

    public List<HitData> GunShot(Vector2 shotAim)
    {
        if (magazineManager.isReloading)
            return null;

        if (isShooting) return null;

        isShooting = true;

        List<HitData> hitDatas = GatherHitData(shotAim);

        if (hitDatas == null) return null;
        if (hitDatas.Count == 0) return null;


        if (shotConnected(hitDatas))
            return hitDatas;


        isShooting = false;
        return null;
    }
    private void CreateReserveBulletLines()
    {
        bulletLinesList = new List<LineRenderer>();
        bulletLinesList.Clear();

        for (int i = 0; i < reserveBulletLineNum; i++)
        {
            LineRenderer newBulletLine = Instantiate(bulletLinePrefab, bulletLinesHolder).GetComponent<LineRenderer>();
            bulletLinesList.Add(newBulletLine);
        }

        StartingPos0bulletTrail = bulletLinesList[0].GetPosition(0);
    }
    private void activeBulletLIne(List<HitData> hitDatas)
    {
        

        foreach (var hitData in hitDatas)
        {
            currentBulletLineIndex = (currentBulletLineIndex +1 ) % (reserveBulletLineNum - 1);
            StartCoroutine(activeBulletLIneCo(hitData.hitPos));
            
        }
    }
    private void activeBulletLIneAirHit(List<HitData> hitDatas)
    {
        

        foreach (var hitData in hitDatas)
        {
            Debug.Log(hitData.hitPos);
            currentBulletLineIndex = (currentBulletLineIndex +1 ) % (reserveBulletLineNum - 1);
            StartCoroutine(activeBulletLIneCo(hitData.hitPos));
            
        }
    }

    private IEnumerator activeBulletLIneCo(Vector3 hitPos)
    {
        LineRenderer bulletLineRenderer = bulletLinesList[currentBulletLineIndex];
        

        bulletLineRenderer.SetPosition(0, StartingPos0bulletTrail);
        bulletLineRenderer.SetPosition(1, hitPos);

        float elaps = 0;
        while (elaps < bulletLineShowTime)
        {
            yield return null;
            elaps += Time.deltaTime;
            
            float t = (elaps /  bulletLineShowTime);



            bulletLineRenderer.SetPosition(0, Vector3.Lerp(StartingPos0bulletTrail, hitPos, t));
        }

        bulletLineRenderer.SetPosition(0, StartingPos0bulletTrail);
        bulletLineRenderer.SetPosition(1, StartingPos0bulletTrail);


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

    private bool shotConnected(List<HitData> hitDatas)
    {

        if (hitDatas == null) return false;
        if (hitDatas.Count == 0) return false;

        foreach (var hit in hitDatas)
        {
            if (hit == null)
            {
                PlayGunShotSfx();
                StartCoroutine(OnFreeSHotAirCo());

                activeBulletLIneAirHit(hitDatas);

                return false;
            }

        }


        HitData magaizne = hitDatas.Find(x => x.hitTYpe == EHitType.Magazine);
        if (magaizne != null)
        {
            Reload();
            return false;
        }

        if (magazineManager.IsEmpty())
        {
            PlayEmptyShotSfx();
            return false;
        }
        else
            PlayGunShotSfx();

        fireCo = StartCoroutine(GunFireCo(hitDatas));
        activeBulletLIne(hitDatas);

        return true;
    }

    private IEnumerator OnFreeSHotAirCo()
    {
        bool isShootingBullet = magazineManager.ConsumeAmmo(1);
        yield return new WaitForSeconds(currentGunData.rapitShootingDelay);
        isShooting = false;

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

        isShooting = false;
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
        Debug.Log(hit.hitTYpe);

        if (hit.hitTYpe == EHitType.Air)
        {
            hit.hitTYpe = EHitType.Missed;
            return;
        }

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

    public void Reload(bool instantReload = false, int reloadAmount = -1)
    {
        if (magazineSocket != null)
            magazineManager.OnReloadBullets(instantReload, reloadAmount, reloadTimeScale);

        isShooting = false;
    }

    public void ReloadBtn()
    {
        Reload(false, -1);  
    }
    

    [ContextMenu("Reset The Gun")]
    public void ResetTheGun()
    {
        Debug.Log("reseted the gun");

        SetCurrentGun(0);
        SetNewMainWeapon(0);
        RemoveSecondaryWeapon();
        isShooting = false;
        reloadTimeScale = 1;
    }


    public void SetNewMainWeapon(int gunId)
    {
        mainWeaponId = gunId;
    }
    public void SetNewScondaryWeapon(int gunId)
    {
        bool usingSecondaryGun = currentGun.gunID == secondaryWeaponId;


        secondaryWeaponId = gunId;

        magazineDisctionary[gunId].CanReload(true);
        magazineDisctionary[gunId].OnReloadBullets(true);
        magazineDisctionary[gunId].CanReload(false);

        hasSecondaryWeapon = true;

        if (usingSecondaryGun)
            SetCurrentGun(gunId, true, false);

        Reload(true);
    }

    public void RemoveSecondaryWeapon()
    {
        if (currentGun.gunID != 0 && currentGun.gunID == secondaryWeaponId)
        {

            SetCurrentGun(mainWeaponId, false, false);
        }

        hasSecondaryWeapon = false;
        secondaryWeaponId = 0;
        player.OnRemoveSecondaryWeapon();
    }

    public void SwitchWeapon()
    {
        
        if (currentGun.gunID == mainWeaponId)
        {

            SetCurrentGun(secondaryWeaponId, false, false);
        }
        else
        {

            SetCurrentGun(mainWeaponId, false, false);
        }
    }

    public void ResetRewards()
    {
        if (currentGun.gunID != 0)
        {
            SetCurrentGun(0, true, true, false);
        }

        SetNewMainWeapon(0);

        RemoveSecondaryWeapon();

       
    }

}
