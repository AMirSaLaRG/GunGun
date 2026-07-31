using System;
using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

public class PlayerController : MonoBehaviour, IDamagable
{
    private TouchControls controls;
    public GameObject TestBullet;
    private MagazineManager magazineManager;

    [Header("Uis")]
    [SerializeField] private PlayerCanvas myCanvas;
    [SerializeField] private UiInGame inGameUi;

    [Header("Setup")]

    [SerializeField] private int healthPointCap = 1;
    [SerializeField] private int hostageKillAlowed = 3;
    [SerializeField] private float comboIntervalCooldown = 2;
    [SerializeField] private float onMoveHitPointBonuse = 1.2f;
    [SerializeField] private float khalasHitPointBonuse = 1.1f;
    


    [Header("GunSetup")]
    [SerializeField] private float gunForce = 100f;
    [SerializeField] private int gunDamage = 1;
    [SerializeField] private GameObject bulletHoldePrefab;
    [SerializeField] private int holeLifetime = 60;
    private int gunAmoCap = 6;

    [SerializeField] private float ComboToPointMultiPlyer = .25f;
    [SerializeField] private LayerMask whatIsUntargetable;

    public Camera mainCamera {  get; private set; }

    private bool gameStarted = false;


    public float points { private set; get; } = 0;
    private int currentKills = 0;
    private int currentCombo = 0;
    private bool isOnCombo = false;
    private int currentAmo;
    private int currentHostageKilled = 0;
    private int healthPoint;

    private float lastComboTime = 0;

    private Enemy lastKill;
    private bool isDead;
    private int khalasShotCount;
    private float currentCahsPoint;
    private void Awake()
    {
        mainCamera = Camera.main;
        controls = new TouchControls();
        magazineManager = GetComponentInChildren<MagazineManager>();


        controls.TouchScreen.Shoot.performed += ctx => OnTap(ctx);

        inGameUi.onReloadBtn += Reload;
    }

    private void Start()
    {
        ResetPlayer();
        UpdateUi();
    }

    private void Update()
    {
        CheckComboTime();
    }



    private void UpdateUi(bool comboChanged = true)
    {
        inGameUi.AmoChange(currentAmo);
        inGameUi.ComboChange(currentCombo);
        inGameUi.KillChange(currentKills);
        inGameUi.PointChange(points);
        inGameUi.UiOnHostageKill(currentHostageKilled);
        if (currentCombo == 0 && comboChanged)
            inGameUi.SetComboTimer(0);
        else if (comboChanged)
            inGameUi.SetComboTimer(comboIntervalCooldown);

        if (currentAmo <= 1)
        {
            myCanvas.OnLowAmo();
        }
        inGameUi.WarningReloadBtn(currentAmo <= 1);

    }

    private void Shoot(Vector2 aimPosition)
    {

        bool shouldReturn = false;

        if (currentAmo <= 0)
        {
            ShootWithEmptyMagazine();
            shouldReturn = true ;
        }

        if (magazineManager.isReloading)
            return;

        if (shouldReturn == false)
        {
            currentAmo--;
        }


        Ray ray = mainCamera.ScreenPointToRay(aimPosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~whatIsUntargetable))
        {
            Debug.Log($"Hit:{hit.collider.name}");
            Vector3 shotPos = hit.point;

            if (hit.collider.TryGetComponent(out MagazineManager magazine) == true)
            {
                Reload();
                return;
            }

            if (shouldReturn)
            {
                shouldReturn = false;
                UpdateUi();
                return;
            }

            GameObject bullet = Instantiate(TestBullet, shotPos, Quaternion.Euler(0, 180, 0));
            magazineManager.OnShotBullet();



            OnHit(hit, shotPos);
        }


    }

    public void Reload()
    {
        magazineManager.OnReloadBullets();
        currentAmo = gunAmoCap;
        myCanvas.OnReload();
        UpdateUi(false);
    }

    private void ShootWithEmptyMagazine()
    {
        Debug.Log("Out Of Amo!!!!");
        magazineManager.OnShotBullet();

    }

    private void MakeHoleOnHit(RaycastHit hit)
    {
        Debug.Log("Created");

        Vector3 placePos = hit.point + (hit.normal * 0.002f);
        Quaternion rotation = Quaternion.LookRotation(hit.normal);

        GameObject hole = Instantiate(bulletHoldePrefab, placePos, rotation);
        hole.transform.parent = hit.transform; // Stick to the surface

        Destroy(hole, holeLifetime); // Clean up after some time
    }

    private void OnHit(RaycastHit hit, Vector3 shotPos)
    {
        Vector3 point = mainCamera.WorldToScreenPoint(hit.point);
        float distance = hit.point.z - transform.position.z;

        if (hit.collider.TryGetComponent(out Target target) == false)
        {
            myCanvas.Onhit(point, distance, EHit.Missed);

            MakeHoleOnHit(hit);

            ResetCombo();
            return;
        }
     
        ShotForce(hit, shotPos, target);

        if (target.TryGetComponent(out Enemy enemy))
            OnEnemyHit(enemy);


        else if (target.TryGetComponent(out Hostage hostage))
        {
            OnHostageHit(hostage);
        }
        else
        {
            myCanvas.Onhit(point, distance, EHit.Missed);
            ResetCombo();
        }

        target.TakeDamage(gunDamage, hit.point);

        UpdateUi();

    }

    public void OnHostageHit(Hostage target)
    {
        Vector3 point = mainCamera.WorldToScreenPoint(target.transform.position);
        float distance = target.transform.position.z - transform.position.z;

        myCanvas.Onhit(point, distance, EHit.Hostage);

        isOnCombo = false;

        if (target.isDead)
            return;

        currentHostageKilled++;


        ResetCombo();

        if (GameManager.instance.isTesting)
        {
            inGameUi.UiOnHostageKill(currentHostageKilled >= hostageKillAlowed ? hostageKillAlowed : currentHostageKilled);
            return;
        }

        inGameUi.UiOnHostageKill(currentHostageKilled);

    

        if (hostageKillAlowed <= currentHostageKilled)
            GameManager.instance.GameOver();
    }

    private void OnEnemyHit(Enemy target)
    {
        Vector3 point = mainCamera.WorldToScreenPoint(target.transform.position);
        float distance = target.transform.position.z - transform.position.z;

        float targetBasePoints = target.GetTargetPoints();

        if (target.isDead)
        {
            khalasShotCount++;



            KhalasComboHandler(targetBasePoints);

            myCanvas.OnHitKhalas(point, distance, khalasShotCount, UnityEngine.Color.orange);
            return;
        }

        lastKill = target;

        khalasShotCount = 0;


        if (target.isMoving)
        {
            myCanvas.Onhit(point, distance, EHit.MovingEnemy);
            targetBasePoints *= onMoveHitPointBonuse;


        } else
        {
            myCanvas.Onhit(point, distance, EHit.Enemy);

        }

        AddPoints(targetBasePoints);
        currentKills++;

        AddToCombo(target.GetComboValue(), target.transform.position);

    }

    private void AddToCombo(int combo, Vector3 targetPos)
    {
        currentCombo += combo;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetPos);

        myCanvas.OnCombo(screenPos, screenPos.z - transform.position.z, currentCombo);

        isOnCombo = true;
        lastComboTime = Time.time;
    }

    private void KhalasComboHandler(float targetPoints)
    {
        targetPoints *= khalasHitPointBonuse;
        AddPoints(targetPoints);

        if (khalasShotCount < 3)
            return;
        if (khalasShotCount < 5)
            AddToCombo(1, lastKill.transform.position);
        if (khalasShotCount >= 5)
            AddToCombo(2, lastKill.transform.position);

    }

    private void AddPoints(float targetPoints)
    {
        points += targetPoints + (targetPoints * currentCombo * ComboToPointMultiPlyer);
        currentCahsPoint += targetPoints + (targetPoints * currentCombo * ComboToPointMultiPlyer);
    }

    private void ResetCombo()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        myCanvas.OnCashPoint(screenPos, 1, currentCahsPoint);
        currentCahsPoint = 0;
        isOnCombo = false;
        currentCombo = 0;
        UpdateUi();

    }

    private void CheckComboTime()
    {
        if (currentCombo == 0) return;
        if (Time.time > lastComboTime + comboIntervalCooldown)
            ResetCombo();
    }

    private void ShotForce(RaycastHit hit, Vector3 shotPos, Target enemy)
    {
        Vector3 hitDirectionNorm = (transform.position - hit.transform.position).normalized;
        enemy.rb.AddForceAtPosition(-gunForce * hitDirectionNorm, shotPos, ForceMode.Impulse);
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        if (gameStarted == false) return;

        StartCoroutine(CheckUITap());
    }

    private IEnumerator CheckUITap()
    {
        // Wait for end of frame so UI state is updated
        yield return new WaitForEndOfFrame();
        // Or yield return null; // Wait one frame

        // Now this will work correctly
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Tapped on UI - ignoring");
            yield break;
        }

        Vector2 aimPos = controls.TouchScreen.Aim.ReadValue<Vector2>();
        Shoot(aimPos);
    }



    private void OnEnable()
    {
        controls?.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void TakeDamage(int damage, Vector3 worldSpaceTakingDamage)
    {
        if (isDead)
            return;

        ResetCombo();

        healthPoint -= damage;
        myCanvas.OnTakingDamage(mainCamera.WorldToScreenPoint(worldSpaceTakingDamage));
        if (healthPoint <= 0)
        {
            if (GameManager.instance.isTesting)
                return;

            isDead = true;
            GameManager.instance.GameOver();
        }

    }

    public void ResetPlayer()
    {
        isDead = false;
        currentHostageKilled = 0;
        currentAmo = gunAmoCap;
        points = 0;
        currentKills = 0;
        currentCombo = 0;
        currentCahsPoint = 0;
        healthPoint = healthPointCap;
        UpdateUi();
        magazineManager.OnReloadBullets();
    }
    public void SetGameStarted(bool gameStarted) => this.gameStarted = gameStarted;

    public int GetKills() => currentKills;
}
