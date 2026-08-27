using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

    [Header("Uis")]
    [SerializeField] private PlayerCanvas myCanvas;
    [SerializeField] private UiInGame inGameUi;

    [Header("Setup")]
    [SerializeField] private int healthPointCap = 1;
    [SerializeField] private float comboIntervalCooldown = 2;
    [SerializeField] private float onMoveHitPointBonuse = 1.2f;
    [SerializeField] private float khalasHitPointBonuse = 1.1f;



    public WeaponData currentGun { private set; get; }
    public Camera mainCamera {  get; private set; }

    private bool gameStarted = false;

    private int healthPoint;

    private bool isDead;
    private bool canTakeDamage = true;
    private HitData hitTaken;

    private void Awake()
    {
        mainCamera = Camera.main;
        controls = new TouchControls();
        gunManager = GetComponent<GunManager>();
        scoreManager = FindFirstObjectByType<ScoreManager>();

        controls.TouchScreen.Shoot.performed += ctx => OnTap(ctx);

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

        hitTaken = hitedData;
        //ResetCombo();

        healthPoint -= damage;
        if (healthPoint <= 0)
        {
            isDead = true;

            if (GameManager.instance.isTesting)
                return;

            GameManager.instance.OnLastChance();

        } else
            myCanvas.OnTakingDamage(mainCamera.WorldToScreenPoint(hitedData.hitPos));


    }


    public void ReviveThePlayer()
    {
        healthPoint = healthPointCap;
        scoreManager.ClearHostageKill();
        myCanvas.OnTakingDamage(mainCamera.WorldToScreenPoint(hitTaken.hitPos));
        hitTaken = null;
        isDead = false;
        canTakeDamage = false;


        Invoke(nameof(CanTakeDamage), 1.5f);
    }

    public void OnGameOver()
    {
        myCanvas.OnTakingDamage(mainCamera.WorldToScreenPoint(hitTaken.hitPos));
        hitTaken = null;
    }
    private void CanTakeDamage () => canTakeDamage = true;
    public void SetGameStarted(bool gameStarted)
    {
        this.gameStarted = gameStarted;
        isDead = !gameStarted;
        canTakeDamage = gameStarted;
        if (gameStarted)
            gunManager.ReadyTheGun();

    }

    public void EquipNewWeapon(int gunId)
    {
        gunManager.SetCurrentGun(gunId);
    }
    internal void WaveChange(float displayTime)
    {
        myCanvas.waveChange(displayTime);
    }
    private void OnEnable()
    {
        controls?.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

  
}
