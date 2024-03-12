using UnityEngine;

public class Gun : MonoBehaviour
{
   public bool isGrabbed = false;
   
   [SerializeField] private GameObject _bullet;
   [SerializeField] private float _fireCountDown = 0.25f;
   [SerializeField] private AudioSource _fireBulletAudio;
   [SerializeField] private Rigidbody _rigidbody;

   private float _fireTimer = 0f;

   private void Start()
   {
      // set up listener for when player tries to fire a bullet
      GameObject.FindGameObjectWithTag("Player").GetComponent<Fire>().fireEvent.AddListener(FireBullet);
   }

   /// <summary>
   /// Fires bullet from a gun if cool time passed and the player is holding the gun.
   /// </summary>
   public void FireBullet()
   {
      if (_fireTimer < _fireCountDown || !isGrabbed) return;
      
      Instantiate(_bullet, transform.position - transform.forward, transform.rotation);
      _fireBulletAudio.Play();
      _fireTimer = 0f;
      _rigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
   }

   private void Update()
   {
      _fireTimer += Time.deltaTime;
   }
}
