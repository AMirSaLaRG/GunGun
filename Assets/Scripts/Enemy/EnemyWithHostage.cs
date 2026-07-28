using System;
using UnityEngine;

public class EnemyWithHostage : Enemy
{
    protected Hostage myHostage;
    [Header("EnemyWIthHostageSetup")]
    [SerializeField] protected float distanceByHostage;

    protected override void Start()
    {
        base.Start();
        moveSpeed = .8f * myHostage.GetMoveSpeed();

        
    }

    protected override void Update()
    {

        base.Update();
    }

    public void Setup(Hostage hostage)
    {
        myHostage = hostage;
    }

    protected override void Shoot()
    {
        anim.SetTrigger(triggerAnimAttackKeyWord);

        player.OnHostageHit(myHostage);

        myHostage.TakeDamage(damage, myHostage.transform.position);
    }

    protected override void AtDieAction()
    {
        base.AtDieAction();
        myHostage.TriggerAtEndOfDuration();
    }

    protected override void OnEnteringViewTracker(Vector3 centerOfTracker)
    {
        base.OnEnteringViewTracker(centerOfTracker);

    }

    public void AtHostageDeath()
    {
        AtEndOfDuration();
    }
    private bool DistanceCheckBy(Transform target) => Vector3.Distance(transform.position, target.position) > distanceByHostage;
    
}
