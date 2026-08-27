using DG.Tweening;
using UnityEngine;

public class Hostage : Target
{

    [Header("HostageSetup")]
    [SerializeField] private GameObject onDeathVfx;
    protected EnemyWithHostage myTaker;
    protected string triggerAnimSurvivedKeyWord = "Survived";

    private Vector3 standPointPos;

    protected override void Start()
    {
        base.Start();
        if (myBox != null)
        {
            if (myTaker != null)
            {
                myWayPoints = myBox.GetSecondaryWayPoints();
                transform.position = myWayPoints[0].position;
                MoveByWayPoints();
               
            }

        }
 
        //else
        //    Debug.Log("Set Hostage Stand Point");

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
                myBox.RespawnLeftTheBox(this);

        transform.DOKill();
    }

    //protected override void AtFinalPositionAction()
    //{
    //    if (myTaker == null)
    //        base.AtFinalPositionAction();
    //    else
    //    {
    //        Vector3 targetPos = new Vector3(standPointPos.x, transform.position.y, standPointPos.z);
    //        float distance = Vector3.Distance(transform.position, targetPos);
    //        float duration = distance / moveSpeed;

    //        transform.DOMove(targetPos, duration).SetEase(Ease.Linear).OnComplete(() =>
    //        {
    //            base.AtFinalPositionAction();
    //        });
    //    }
    //}

    protected override void AtDieAction()
    {

        if (myTaker != null)
        {
            myTaker.AtHostageDeath();
            myBox = null;
        }

        onDeathVfx.SetActive(true);
        onDeathVfx.transform.parent = null;

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

}
