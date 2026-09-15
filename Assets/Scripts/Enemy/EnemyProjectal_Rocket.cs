using UnityEngine;

public class EnemyProjectal_Rocket : EnemyProjectal
{
    [Header("Rocket")]
    [SerializeField] private Transform projectile;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private float orbitRadius = 0.5f;
    [SerializeField] private float orbitSpeed = 250;   // degrees per second
    [SerializeField] private Vector3 orbitAxis = Vector3.forward; // axis in local space
    private float orbitAngle;

    protected override void Update()
    {
        base.Update();

        orbitAngle += orbitSpeed * Time.deltaTime;
        if (orbitAngle > 360f) orbitAngle -= 360f;

        // rotate an offset around the chosen axis, then apply it as local position
        Quaternion rot = Quaternion.AngleAxis(orbitAngle, orbitAxis.normalized);
        projectile.localPosition = rot * (Vector3.right * orbitRadius);
    }

    protected override void PlayVfx()
    {
        AudioManager.instance.PlaySfx(sfx, true);
        Instantiate(onHitVfx.gameObject, projectile.position, Quaternion.identity, null);

    }



    

}
