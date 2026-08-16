using UnityEngine;

[System.Serializable]
public class HitData
{
    public EHitType hitTYpe;
    public Vector3 hitPos;
    public Target hitedTarget;
    public Vector3 hitNorm;
    public Transform hitTransform;
    public float hitForce;

    public HitData Copy()
    {
        return new HitData
        {
            hitTYpe = this.hitTYpe,
            hitPos = this.hitPos,
            hitedTarget = this.hitedTarget,
            hitNorm = this.hitNorm,
            hitTransform = this.hitTransform,
            hitForce = this.hitForce,
        };
    }
}
