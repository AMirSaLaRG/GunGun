using DG.Tweening;
using UnityEngine;

public class PowerUp_Target : Target
{
    [Header("PowerUp")]
    [SerializeField] private Drone_PowerUp myPowerUp;


    protected override void AtDieAction()
    {
        base.AtDieAction();
        myPowerUp.Activate();
        myPowerUp.transform.DOKill();
        Destroy(myPowerUp.gameObject);
    }
}
