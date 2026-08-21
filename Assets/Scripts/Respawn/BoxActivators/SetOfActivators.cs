using UnityEngine;

public class SetOfActivators : EntityActivator
{
    [SerializeField] private EntityActivator[] myActivators;



    public override void SetActive(out float activateTime)
    {
        activateTime = 0;
        if (isActive)
            return;
        foreach (var activator in myActivators)
        {
            activator.SetActive(out float individualActivateTime);
            activateTime = Mathf.Max(activateTime, individualActivateTime);
        }
        isActive = true;
    }

    public override void SetDeActive()
    {
        foreach (var activator in myActivators)
        {
            activator.SetDeActive();
        }
        isActive = false;
    }

   
}
