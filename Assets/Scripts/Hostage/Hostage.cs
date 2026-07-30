using DG.Tweening;
using UnityEngine;

public class Hostage : Target
{
    protected EnemyWithHostage myTaker;
    protected Transform myStandPoint;
    protected string triggerAnimSurvivedKeyWord = "Survived";

    private Vector3 standPointPos;

    protected override void Start()
    {
        base.Start();
        if (myBox != null)
            myStandPoint = myBox.GetHostagePoint();
        if (myStandPoint != null)
            standPointPos = myStandPoint.position;
        else
            Debug.Log("Set Hostage Stand Point");

    }
    protected override void AtEndOfDuration()
    {
        Destroy(rb);
        Destroy(mycollider);
        anim.SetTrigger(triggerAnimSurvivedKeyWord);

        AtEndOfDurationAction();
        Destroy(gameObject, 1f);
    }

    protected override void AtEndOfDurationAction()
    {
        if (myTaker == null)
            if (myBox != null)
                myBox.MakeEmpty();

        transform.DOKill();
    }

    protected override void OnEnteringViewTracker(Vector3 centerPoint)
    {

        if (myTaker == null)
        {
            base.OnEnteringViewTracker(centerPoint);
        } else
        {
            Vector3 targetPos = new Vector3(standPointPos.x, transform.position.y, standPointPos.z);
            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / moveSpeed;

            transform.DOMove(targetPos, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                base.OnEnteringViewTracker(centerPoint);
            });
        }
            
    }
    protected override void AtDieAction()
    {

        if (myTaker != null)
        {
            myTaker.AtHostageDeath();
            myBox = null;
        }

        base.AtDieAction();

 
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

    public void SetStandPod(Transform standPoint) => myStandPoint = standPoint;
}
