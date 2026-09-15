using UnityEngine;

public class EnemyProjectal : MonoBehaviour
{
    [Header("setup")]
    [SerializeField] protected ParticleSystem onHitVfx;
    [SerializeField] private float disFromCameraToHit = 3f;
    private float speed;
    private int damage;
    private PlayerController player;
    private Transform target;
    private float targetZ;
    private bool fromMinusComesToTarget;
    private bool isDistanceEnough = false;

    private void Start()
    {
        target = player.mainCamera.transform;

        transform.LookAt(target.position);

        if (transform.position.z < target.position.z)
            fromMinusComesToTarget = true;
        else
            fromMinusComesToTarget = false; 

        if (fromMinusComesToTarget)
            targetZ = target.position.z - disFromCameraToHit;
        else
            targetZ = target.position.z + disFromCameraToHit;
    }

    protected virtual void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (fromMinusComesToTarget)
            isDistanceEnough = transform.position.z > targetZ;
        else
            isDistanceEnough = transform.position.z < targetZ;


        if (isDistanceEnough)
        {
            OnHitPlayer();
        }
    }

    private void OnHitPlayer()
    {
        HitData hitData = new HitData();
        hitData.hitPos = transform.position;

        player.TakeDamage(damage, hitData);
        DestroyTheProjectile();
    }

    protected virtual void PlayVfx()
    {
        Instantiate(onHitVfx.gameObject, transform.position, Quaternion.identity, null);
    }

    public void SetUP(float speed, int damage, PlayerController player)
    {
        this.speed = speed;
        this.player = player;
        this.damage = damage;
    }

    public void DestroyTheProjectile()
    {
        PlayVfx();
        Destroy(gameObject);
    }
}
