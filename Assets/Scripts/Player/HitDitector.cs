using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HitDitector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask whatIsUntargetable;
    [SerializeField] private float acuracyThreshold = 100;


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

        if (hit.collider == null)
            return AirHit();


        return null;
    }

    private HitData AirHit()
    {
        HitData hit = new HitData()
        {
            hitTYpe = EHitType.Missed,
            hitPos = new Vector3(-99999, -999999, -999999),

        };

        return hit;

    }

    public List<HitData> MultiShot(Vector2 shotAim, float acuracy, int shots)
    {
        float shotThreshold = Mathf.Lerp(acuracyThreshold, 0, acuracy);

        List<HitData> returnList = new List<HitData>();

        HitData mainHit = SingleShot(shotAim);
        if (mainHit == null)
            return null;

        float extraHit = shots - 1;

        if (mainHit.hitedTarget != null && mainHit.hitedTarget.TryGetComponent(out Enemy enemy))
        {
            returnList.Add(mainHit);

            for (int i = 0; i < extraHit; i++)
            {
                HitData extraHitData = mainHit.Copy();

                extraHitData.hitPos = mainHit.hitedTarget.GetRandomPointOnCollider();
                extraHitData.hitForce *= 0.1f;

                returnList.Add(extraHitData);
            }

            return returnList;
        }

        else if (mainHit.hitedTarget != null && mainHit.hitedTarget.TryGetComponent(out EnemyShield enemyShield))
        {
            returnList.Add(mainHit);

            for (int i = 0; i < extraHit; i++)
            {
                HitData extraHitData = mainHit.Copy();

                extraHitData.hitedTarget = enemyShield.GetHolder();
                extraHitData.hitPos = enemyShield.GetHolder().GetRandomPointOnCollider();
                extraHitData.hitForce *= 0.5f;

                returnList.Add(extraHitData);
            }

            return returnList;
        }
        else if (mainHit.hitedTarget != null && mainHit.hitedTarget.TryGetComponent(out Hostage hostage))
        {
            // Convert LayerMask to layer index
            // REMEMBERR
            int untargetableLayer = (int)Mathf.Log(whatIsUntargetable.value, 2);

            hostage.gameObject.layer = untargetableLayer;
        }


        returnList.Add(mainHit);

        for (int i = 0; i < extraHit; i++)
        {
            float x = UnityEngine.Random.Range(-shotThreshold, shotThreshold);
            float y = UnityEngine.Random.Range(-shotThreshold, shotThreshold);

            HitData newHit = SingleShot(new Vector2(shotAim.x + x, shotAim.y + y));

            returnList.Add(newHit);
        }

        return returnList;

        
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
            hitData.hitTYpe = GetTargetStatus(target);
        }


        return hitData;
    }

    public EHitType GetTargetStatus(Target target)
    {
        if (target == null)
            return EHitType.None;

        if (target.TryGetComponent(out Enemy enemy))
        {
            if (enemy.isMoving)
                return EHitType.MovingEnemy;
            else if (enemy.isDead)
                return EHitType.Khalas;
            else
                return EHitType.Enemy;
        }

        else if (target.TryGetComponent(out EnemyShield enemyShiedl))
        {
            if (enemyShiedl.isDead == true)
                return EHitType.Enemy;
            else
                return EHitType.Shield;
        }

        else if (target.TryGetComponent(out Hostage hostage))
        {
            if (hostage.isDead)
                return EHitType.Missed;
            else
                return EHitType.Hostage;
        }
        else if (target.TryGetComponent(out TargetableProjectile rocket))
        {
            
            return EHitType.EnemyProjectile;
    
        }

      

        return EHitType.None;

    }

}

