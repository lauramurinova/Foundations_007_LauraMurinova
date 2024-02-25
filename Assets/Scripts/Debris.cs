using UnityEngine;

public class Debris : MonoBehaviour
{
    [SerializeField] private GameObject hitParticleSystemPrefab;
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private float collisionImpulseThreshold = 1.5f;
    [SerializeField] private float particleScaleFactor = 0.1f;
    [SerializeField] private float audioVolumeFactor = 0.05f;
    
    private void OnCollisionEnter(Collision collision)
    {
        // Only create a collision effect, if the impact was the min threshold.
        if (collision.impulse.magnitude > collisionImpulseThreshold)
        {
            CreateCollisionEffect(collision);
        }
    }
    
    /// <summary>
    /// Ensures a hit particle and sound effect are created on collision of the object.
    /// </summary>
    private void CreateCollisionEffect(Collision collision)
    {
        var particleSystemObject = Instantiate(
            hitParticleSystemPrefab,
            GetCenterPoint(collision),
            Quaternion.identity,
            transform
        );
        
        // Adjust the particle size and volume based on the impact.
        particleSystemObject.transform.localScale *= particleScaleFactor * collision.impulse.magnitude;
        hitAudioSource.volume = audioVolumeFactor * collision.impulse.magnitude;
        hitAudioSource.Play();
    }

    /// <summary>
    /// Calculates the average center point of all contact points and returns it.
    /// </summary>
    private static Vector3 GetCenterPoint(Collision collision)
    {
        Vector3 centerPoint = Vector3.zero;

        foreach (ContactPoint contactPoint in collision.contacts)
        {
            centerPoint += contactPoint.point;
        }

        centerPoint /= collision.contacts.Length;
        return centerPoint;
    }
}
