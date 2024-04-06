using UnityEngine;

public enum Handed
{
    Left = 0,
    Right = 1
}

public class Handedness : MonoBehaviour
{
    public Handed handed;

    [SerializeField] private GameEventManager _gameManager;
    [SerializeField] private GameObject[] _leftHandedObjects;
    [SerializeField] private GameObject[] _rightHandedObjects;

    private void Start()
    {
        handed = _gameManager.handedness;
        
        if (handed == Handed.Left)
        {
            foreach (var obj in _leftHandedObjects)
            {
                obj.SetActive(true);
            }
            
            foreach (var obj in _rightHandedObjects)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (var obj in _leftHandedObjects)
            {
                obj.SetActive(false);
            }
            
            foreach (var obj in _rightHandedObjects)
            {
                obj.SetActive(true);
            }
        }
    }
}
