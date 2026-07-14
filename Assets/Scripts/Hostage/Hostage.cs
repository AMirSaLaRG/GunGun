using UnityEngine;

public class Hostage : Target
{
    protected string triggerAnimSurvivedKeyWord = "Survived";

    protected override void AtEndOfDuration()
    {
        Destroy(rb);
        Destroy(mycollider);
        anim.SetTrigger(triggerAnimSurvivedKeyWord);

        Invoke(nameof(AtEndOfDurationAction), .5f);
        Destroy(gameObject, 1);

    }

}
