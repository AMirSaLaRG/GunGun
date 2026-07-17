using UnityEngine;

public class EnemyProjectal : MonoBehaviour
{
    [Header("setup")]
    [SerializeField] private ParticleSystem onHitVfx;
    private float speed;
    private int damage;
    private PlayerController player;
    


    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (transform.position.z < Camera.main.transform.position.z + 1)
        {
            Instantiate(onHitVfx.gameObject, transform.position, Quaternion.identity, null);
            player.TakeDamage(damage);
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
