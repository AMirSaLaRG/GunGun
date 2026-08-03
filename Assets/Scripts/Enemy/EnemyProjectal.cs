using UnityEngine;

public class EnemyProjectal : MonoBehaviour
{
    [Header("setup")]
    [SerializeField] private ParticleSystem onHitVfx;
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

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (fromMinusComesToTarget)
            isDistanceEnough = transform.position.z > targetZ;
        else
            isDistanceEnough = transform.position.z < targetZ;


        if (isDistanceEnough)
        {
            Instantiate(onHitVfx.gameObject, transform.position, Quaternion.identity, null);
            player.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }

    public void SetUP(float speed, int damage, PlayerController player)
    {
        this.speed = speed;
        this.player = player;
        this.damage = damage;
    }
}
