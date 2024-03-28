using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out EnemyController enemy))
        {
            enemy.SetStunned();
            Destroy(gameObject);
        }
    }
}
