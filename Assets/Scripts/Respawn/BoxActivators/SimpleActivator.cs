using UnityEngine;

public class SimpleActivator : EntityActivator
{
    [SerializeField] private GameObject[] ActivateItems;



    public override void SetActive(out float activateTime)
    {
        foreach (var item in ActivateItems)
            item.SetActive(true);
        base.SetActive(out activateTime);
    }

    public override void SetDeActive()
    {
        base.SetDeActive();
        foreach (var item in ActivateItems)
            item.SetActive(false);
    }
}
