using System;
using UnityEngine;

public class EnemyWithHostage : Enemy
{
    protected Hostage myHostage;
    protected ScoreManager scoreManager;
    [Header("EnemyWIthHostageSetup")]
    [SerializeField] protected float distanceByHostage;

    protected override void Start()
    {
        base.Start();

        facingPosition = myBox.GetHostagePoint().position;

        SpeedUpTarget(0, BaseMoveSpeed, .5f);

        scoreManager = player.scoreManager;

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

        scoreManager.OnHostageKilled();

        HitData hitData = new HitData();
        hitData.hitPos = myHostage.transform.position;

        myHostage.TakeDamage(damage, hitData);
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
