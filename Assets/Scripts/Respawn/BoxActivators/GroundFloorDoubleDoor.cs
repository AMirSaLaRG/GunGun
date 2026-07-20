using DG.Tweening;
using UnityEngine;

public class GroundFloorDoubleDoor : EntityActivator
{
    [SerializeField] protected Transform myActivateItem2;

    public override void SetActive()
    {
        base.SetActive();

        Vector3 openScaleV3 = new Vector3(scaleDirection.x == 0 ? closeScale.x : openScale,
            scaleDirection.y == 0 ? closeScale.y : openScale,
            scaleDirection.z == 0 ? closeScale.z : openScale);

        myActivateItem2.DOScale(openScaleV3, activateTransitionTIme).SetEase(ActivateEase);
    }

    public override void SetDeActive()
    {
        base.SetDeActive();

        myActivateItem2.DOScale(closeScale, activateTransitionTIme);

    }
}
