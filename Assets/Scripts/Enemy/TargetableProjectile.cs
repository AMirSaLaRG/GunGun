using UnityEngine;

public class TargetableProjectile : Target
{
    public EnemyProjectal myRoot;

    protected override void Start()
    {
        isStatic = true;
        base.Start();
    }

    public void GotHit()
    {
        myRoot.DestroyTheProjectile();
    }

    protected override void AtDieAction()
    {
        GotHit();
    }
}
