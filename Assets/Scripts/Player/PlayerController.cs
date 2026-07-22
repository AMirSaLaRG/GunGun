using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamagable
{
    private TouchControls controls;
    private UiManager uiManager;
    public GameObject TestBullet;
    private MagazineManager magazineManager;


    [Header("Setup")]

    [SerializeField] private int healthPointCap = 1;
    [SerializeField] private int hostageKillAlowed = 3;
    [SerializeField] private float comboIntervalCooldown = 2;


    [Header("GunSetup")]
    [SerializeField] private float gunForce = 100f;
    [SerializeField] private int gunDamage = 1;
    private int gunAmoCap = 6;

    [SerializeField] private float ComboToPointMultiPlyer = .25f;
    [SerializeField] private LayerMask whatIsUntargetable;

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
    private void Awake()
    {
        controls = new TouchControls();
        uiManager = FindFirstObjectByType<UiManager>();
        magazineManager = GetComponentInChildren<MagazineManager>();


        controls.TouchScreen.Shoot.performed += ctx => OnTap(ctx);

        uiManager.onReloadBtn += Reload;
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



    private void UpdateUi()
    {
        uiManager.AmoChange(currentAmo);
        uiManager.ComboChange(currentCombo);
        uiManager.KillChange(currentKills);
        uiManager.PointChange(points);
        uiManager.UiOnHostageKill(currentHostageKilled);
        if (currentCombo == 0)
            uiManager.SetComboTimer(0);
        else
            uiManager.SetComboTimer(comboIntervalCooldown);

        uiManager.WarningReloadBtn(currentAmo <= 1);

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
            UpdateUi();
        }


        Ray ray = Camera.main.ScreenPointToRay(aimPosition);
        
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
        UpdateUi();
    }

    private void ShootWithEmptyMagazine()
    {
        Debug.Log("Out Of Amo!!!!");
        magazineManager.OnShotBullet();

    }

    private void OnHit(RaycastHit hit, Vector3 shotPos)
    {
  

        if (hit.collider.TryGetComponent(out Target target) == false)
        {
            ResetCombo();
            return;
        }
     
        ShotForce(hit, shotPos, target);

        if (target.TryGetComponent(out Enemy enemy))
            OnEnemyHit(enemy);

        else if (target.TryGetComponent(out Hostage hostage))
            OnHostageHit(hostage);
        else
            ResetCombo();

        target.TakeDamage(gunDamage);

        UpdateUi();

    }

    private void OnHostageHit(Hostage target)
    {
        isOnCombo = false;

        if (target.isDead)
            return;

        currentHostageKilled++;


        ResetCombo();

        uiManager.UiOnHostageKill(currentHostageKilled);

        if (hostageKillAlowed <= currentHostageKilled)
            GameManager.instance.GameOver();
    }

    private void OnEnemyHit(Enemy target)
    {
        if (target.isDead)
            return;
        if(lastKill == target)
            return;

        lastKill = target;
        

        float targetBasePoints = target.GetTargetPoints();
        points += targetBasePoints + (targetBasePoints * currentCombo * ComboToPointMultiPlyer);
        currentKills++;

        AddToCombo(target.GetComboValue());

    }

    private void AddToCombo(int combo = 1)
    {
        currentCombo += combo;
        isOnCombo = true;
        lastComboTime = Time.time;
    }

    private void ResetCombo()
    {
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

    public void TakeDamage(int damage)
    {
        healthPoint -= damage;
        if (healthPoint <= 0)
            GameManager.instance.GameOver();

    }

    public void ResetPlayer()
    {
        currentHostageKilled = 0;
        currentAmo = gunAmoCap;
        points = 0;
        currentKills = 0;
        currentCombo = 0;
        healthPoint = healthPointCap;
        UpdateUi();
    }
    public void SetGameStarted(bool gameStarted) => this.gameStarted = gameStarted;
}
