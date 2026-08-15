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
}
