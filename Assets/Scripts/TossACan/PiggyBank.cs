using UnityEngine;

public class PiggyBank : MonoBehaviour
{
    [SerializeField] private TossACanManager _tossACanManager;
    [SerializeField] private AudioSource _coinDrop;

    private void OnTriggerEnter(Collider other)
    {
        // Resets the game if coin was inserted.
        if (other.GetComponent<Coin>())
        {
            _coinDrop.Play();
            _tossACanManager.ResetGame();
            Destroy(other.gameObject);
        }
    }
}
