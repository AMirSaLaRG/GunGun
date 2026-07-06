using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamagable
{
    private TouchControls controls;
    public GameObject TestBullet;

    [Header("Setup")]
    [SerializeField] private int healthPoint = 1;
    [SerializeField] private int hostageKillAlowed = 3;

    [Header("GunSetup")]
    [SerializeField] private float gunForce = 100f;
    [SerializeField] private int gunDamage = 1;
    [SerializeField] private float ComboToPointMultiPlyer = .25f;
    [SerializeField] private LayerMask whatIsUntargetable;


    private float points = 0;
    private int currentCombo = 0;
    private bool isOnCombo = false;


    private Enemy lastKill;
    private void Awake()
    {
        controls = new TouchControls();


        controls.TouchScreen.Shoot.performed += ctx => OnShoot(ctx);
    }

    private void Shoot(Vector2 aimPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(aimPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~whatIsUntargetable))
        {
            Debug.Log($"Hit:{hit.collider.name}");
            Vector3 shotPos = hit.point;
            GameObject bullet = Instantiate(TestBullet, shotPos, Quaternion.Euler(0, 180, 0));

            OnHit(hit, shotPos);
        }


    }

    private void OnHit(RaycastHit hit, Vector3 shotPos)
    {
        if (hit.collider.TryGetComponent(out Target target) == false)
        {
            ResetCombo();
            return;
        }
     
        target.TakeDamage(gunDamage);
        ShotForce(hit, shotPos, target);

        if (target.TryGetComponent(out Enemy enemy))
            OnEnemyHit(enemy);

        else if (target.TryGetComponent(out Hostage hostage))
            OnHostageHit(hostage);
        else
            ResetCombo();

    }

    private void OnHostageHit(Hostage target)
    {
        isOnCombo = false;

        if (target.isDead == false)
            return;

        ResetCombo();

        hostageKillAlowed--;

        if (hostageKillAlowed <= 0)
            Debug.Log("GameOver you killed alot of hostaged you are fired!");
    }

    private void OnEnemyHit(Enemy target)
    {
        if (target.isDead == false)
            return;
        if(lastKill == target)
            return;

        lastKill = target;

        float targetBasePoints = target.GetTargetPoints();
        points += targetBasePoints + (targetBasePoints * currentCombo * ComboToPointMultiPlyer);

        AddToCombo(target.GetComboValue());

    }

    private void AddToCombo(int combo = 1)
    {
        currentCombo += combo;
        isOnCombo = true;
    }

    private void ResetCombo()
    {
        isOnCombo = false;
        currentCombo = 0;
    }

    private void ShotForce(RaycastHit hit, Vector3 shotPos, Target enemy)
    {
        Vector3 hitDirectionNorm = (transform.position - hit.transform.position).normalized;
        enemy.rb.AddForceAtPosition(-gunForce * hitDirectionNorm, shotPos, ForceMode.Impulse);
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
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
            Debug.Log("You took shot you are dead");
    }
}
