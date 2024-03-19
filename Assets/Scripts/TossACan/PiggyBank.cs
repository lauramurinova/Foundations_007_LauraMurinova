using UnityEngine;

public class PiggyBank : MonoBehaviour
{
    [SerializeField] private CarnivalActivity _activityManager;
    [SerializeField] private AudioSource _coinDrop;

    private void OnTriggerEnter(Collider other)
    {
        // Resets the given game if coin was inserted.
        if (other.GetComponent<Coin>())
        {
            _coinDrop.Play();
            _activityManager.ResetGame();
            Destroy(other.gameObject);
        }
    }
}
