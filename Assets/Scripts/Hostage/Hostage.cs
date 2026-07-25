using DG.Tweening;
using UnityEngine;

public class Hostage : Target
{
    protected EnemyWithHostage myTaker;
    protected string triggerAnimSurvivedKeyWord = "Survived";

    protected override void AtEndOfDuration()
    {
        Destroy(rb);
        Destroy(mycollider);
        anim.SetTrigger(triggerAnimSurvivedKeyWord);

        AtEndOfDurationAction();
        Destroy(gameObject, 1f);
    }

    protected override void OnEnteringViewTracker(Vector3 centerPoint)
    {
        transform.DOMove(new Vector3 (centerPoint.x, transform.position.y, centerPoint.z), 0.3f).OnComplete(() =>
        {
            base.OnEnteringViewTracker(centerPoint);
        });
    }
    protected override void AtDieAction()
    {
        myRespawnManager = null;
        base.AtDieAction();
        myTaker.AtHostageDeath();
    }
    public void Setup(EnemyWithHostage taker)
    {
        myTaker = taker;
    }

    public void TriggerAtEndOfDuration()
    {
        isMoving = false;
        AtEndOfDuration();
    }
}
