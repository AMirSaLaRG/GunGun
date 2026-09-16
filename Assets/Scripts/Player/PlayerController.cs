using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using static UnityEngine.Analytics.IAnalytic;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour, IDamagable
{
    private TouchControls controls;
    public GunManager gunManager {private set; get; }   
    public ScoreManager scoreManager {  get; private set; }
    public PlayerSkills skills {  get; private set; }

    [Header("Uis")]
    [SerializeField] private PlayerCanvas myCanvas;
    [SerializeField] private UiInGame inGameUi;

    [Header("Setup")]
    [SerializeField] private int healthPointCap = 1;
    [SerializeField] private float comboIntervalCooldown = 2;
    [SerializeField] private float onMoveHitPointBonuse = 1.2f;
    [SerializeField] private float khalasHitPointBonuse = 1.1f;
    [SerializeField] private float ImunityTimeAfterRevive  = 4;



    public Camera mainCamera {  get; private set; }

    private bool gameStarted = false;

    private int healthPoint;

    private bool isDead;
    private bool canTakeDamage = true;

    private void Awake()
    {
        mainCamera = Camera.main;
        controls = new TouchControls();
        gunManager = GetComponent<GunManager>();
        skills = GetComponent<PlayerSkills>();
        scoreManager = FindFirstObjectByType<ScoreManager>();

        controls.TouchScreen.Shoot.performed += ctx => OnTap(ctx);

    }

    private void Start()
    {
        healthPoint = healthPointCap;
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        if (GameManager.instance.gameState != EGameState.InGame) return;

        StartCoroutine(CheckScreenTouch());
    }

    private IEnumerator CheckScreenTouch()
    {
        yield return new WaitForEndOfFrame();

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("Tapped on UI - ignoring");
            yield break;
        }

        Vector2 aimPos = controls.TouchScreen.Aim.ReadValue<Vector2>();
        gunManager.GunShot(aimPos);
    }

    public void HandleShot(HitData hitData)
    {
        if (hitData == null) return;

        scoreManager.ProcessHit(hitData);
        myCanvas.Onhit(mainCamera, hitData);
    }

    public void TakeDamage(int damage, HitData hitedData)
    {
        if (isDead)
            return;

        if (canTakeDamage == false)
            return;

        if (GameManager.instance.isTesting)
            return;

        healthPoint -= damage;
        myCanvas.OnTakingDamage(mainCamera.WorldToScreenPoint(hitedData.hitPos));

        if (healthPoint <= 0)
        {
            isDead = true;
            GameManager.instance.OnLastChance();
        }
    }


    public void ReviveThePlayer()
    {
        healthPoint = healthPointCap;
        myCanvas.ShakeScreen();
        myCanvas.OnImunity(ImunityTimeAfterRevive);
        isDead = false;
        canTakeDamage = false;


        Invoke(nameof(RemoveImunity), ImunityTimeAfterRevive);
    }

    public void OnGameOver()
    {
        myCanvas.ShakeScreen();
    }
    private void RemoveImunity()
    {
        canTakeDamage = true;
    }
    public void SetGameStarted(bool gameStarted)
    {
        this.gameStarted = gameStarted;
        isDead = !gameStarted;
        canTakeDamage = gameStarted;
        skills.ResetSkills();
        if (gameStarted)
            gunManager.ResetTheGun();

    }

    public void EquipNewWeapon(int gunId, bool isInstant = false, bool maxAmo = true, bool asMain = true)
    {
        gunManager.SetCurrentGun(gunId, isInstant, maxAmo);

        if (asMain)
            gunManager.SetNewMainWeapon(gunId);
        else
            gunManager.SetNewScondaryWeapon(gunId);
    }

    [ContextMenu("Switch Weapon")]

    public void SwitchWeapon()
    {
        Sprite gunIcon;

        gunIcon = gunManager.GetGunData(gunManager.currentGun.gunID).gunIcon;

        gunManager.SwitchWeapon();


        myCanvas.ShowSwitchBtn(true, gunIcon);

    }
    internal void SetSecondaryWeapon(int gunId)
    {
        gunManager.SetNewScondaryWeapon(gunId);

        Sprite gunIcon = gunManager.GetGunData(gunId).gunIcon;

        myCanvas.ShowSwitchBtn(true, gunIcon);
    }
    public void OnRemoveSecondaryWeapon()
    {
        myCanvas.ShowSwitchBtn(false, null);


    }

    [ContextMenu("Set secondary weapon")]
    public void SetSecondaryWeapon()
    {
        SetSecondaryWeapon(3);
    }

    public void ResetRewards()
    {

        gunManager.ResetRewards();
        
        
    }

    internal void WaveChange(WaveData data, string waveNum)
    {
        myCanvas.waveChange(data, waveNum);
    }
    private void OnEnable()
    {
        controls?.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void ScaleReloadTime(float scale)
    {
        gunManager.ChangeReloadTimeScale(scale);
    }


    public void OnPause()
    {
        skills.OnPause();
    }

    public void OnResume()
    {
        skills.OnResume();
    }
  
}
