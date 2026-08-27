using UnityEngine;

public class SetOfActivators : EntityActivator
{
    [SerializeField] private EntityActivator[] myActivators;



    public override void SetActive(out float activateTime, float forceTime = 0)
    {

        float duration = forceTime == 0 ? activateTransitionTime : forceTime;

        activateTime = 0;
        if (isActive)
            return;
        foreach (var activator in myActivators)
        {
            activator.SetActive(out float individualActivateTime, duration);
            activateTime = Mathf.Max(activateTime, individualActivateTime);
        }
        isActive = true;
    }

    public override void SetDeActive(bool isReseting = false)
    {
        foreach (var activator in myActivators)
        {
            activator.SetDeActive(isReseting);
        }
        isActive = false;
    }

   
}
