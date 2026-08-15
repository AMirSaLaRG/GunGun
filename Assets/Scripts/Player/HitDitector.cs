using Unity.VisualScripting;
using UnityEngine;

public class HitDitector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask whatIsUntargetable;

    private Camera mainCamera;


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public HitData SingleShot(Vector2 gunAim)
    {
        Ray ray = mainCamera.ScreenPointToRay(gunAim);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~whatIsUntargetable))
            return OnHit(hit);

        return null;
    }

    

    private HitData OnHit(RaycastHit hit)
    {
        HitData hitData = new HitData();

        hitData.hitPos = hit.point;
        hitData.hitedTarget = null;
        hitData.hitNorm = hit.normal;
        hitData.hitTransform = hit.transform;

        if (hit.collider.TryGetComponent(out MagazineManager magazine) == true)
            hitData.hitTYpe = EHitType.Magazine;
        else if (hit.collider.TryGetComponent(out Target target) == false)
            hitData.hitTYpe = EHitType.Missed;
        else
        {
            hitData.hitedTarget = target;

            if (target.TryGetComponent(out Enemy enemy))
            {
                if (enemy.isMoving)
                    hitData.hitTYpe = EHitType.MovingEnemy;
                else if (enemy.isDead)
                    hitData.hitTYpe = EHitType.Khalas;
                else
                    hitData.hitTYpe = EHitType.Enemy;
            }

            else if (target.TryGetComponent(out EnemyShield enemyShiedl))
                hitData.hitTYpe = EHitType.Shield;

            else if (target.TryGetComponent(out Hostage hostage))
                hitData.hitTYpe = EHitType.Hostage;

        }
        

        

        return hitData;
    }
}

