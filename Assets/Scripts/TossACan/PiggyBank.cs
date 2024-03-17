using UnityEngine;

public class PiggyBank : MonoBehaviour
{
    [SerializeField] private TossACanManager _tossACanManager;
    [SerializeField] private AudioSource _coinDrop;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Coin>())
        {
            Debug.Log("RELOAD CALLED");
            _coinDrop.Play();
            _tossACanManager.ResetGame();
            Destroy(other.gameObject);
        }
    }
}
