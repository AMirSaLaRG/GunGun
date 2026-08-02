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
       facingPosition = myBox.GetHostagePoint().position;

    }

    public void Setup(Hostage hostage)
    {
        myHostage = hostage;
        myHostage.SetMyDuration(duration * 1.1f);
    }

    protected override void Shoot()
    {

        AudioManager.instance.PlaySfx(shotSfx, true, true);


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
