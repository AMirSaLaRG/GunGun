using UnityEngine;

public class ViewTracker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyProjectal enemyProjectal))
        {
            Destroy(enemyProjectal);
        }
    }
}
