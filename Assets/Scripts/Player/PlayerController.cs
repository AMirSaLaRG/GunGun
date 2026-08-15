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
    private GunManager gunManager;
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
        if (GameManager.instance.gameState != EGameState.inGame) return;

        StartCoroutine(CheckScreenTouch());
    }

    private IEnumerator CheckScreenTouch()
    {
        yield return new WaitForEndOfFrame();

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Tapped on UI - ignoring");
            yield break;
        }

        Vector2 aimPos = controls.TouchScreen.Aim.ReadValue<Vector2>();
        List<HitData> hitDatas = gunManager.GunShot(aimPos);

        if (hitDatas != null)
        {
            foreach (var hitData in hitDatas)
                scoreManager.ProcessHit(hitData);

            myCanvas.Onhit(mainCamera, hitDatas);

        }
    }

    public void TakeDamage(int damage, HitData hitedData)
    {
        if (isDead)
            return;

        //ResetCombo();

        healthPoint -= damage;
        myCanvas.OnTakingDamage(mainCamera.WorldToScreenPoint(hitedData.hitPos));
        if (healthPoint <= 0)
        {
            if (GameManager.instance.isTesting)
                return;

            isDead = true;
            GameManager.instance.GameOver();
        }

    }


    public void SetGameStarted(bool gameStarted)
    {
        this.gameStarted = gameStarted;
        isDead = !gameStarted;
        if (gameStarted)
            gunManager.ReadyTheGun();

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
