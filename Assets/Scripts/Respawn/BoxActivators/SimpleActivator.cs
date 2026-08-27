using UnityEngine;

public class SimpleActivator : EntityActivator
{
    [SerializeField] private GameObject[] ActivateItems;



    public override void SetActive(out float activateTime, float forceTime = 0)
    {
        float duration = forceTime == 0 ? activateTransitionTime : forceTime;

        foreach (var item in ActivateItems)
            item.SetActive(true);
        base.SetActive(out activateTime, duration);
    }

    public override void SetDeActive(bool isImediatly = false)
    {
        base.SetDeActive(isImediatly);
        foreach (var item in ActivateItems)
            item.SetActive(false);
    }
}
