using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float timeBeforeCollapse = 3f;

    private Rigidbody _rigidBody;
    private bool _isTriggered = false;
    private int playerLayer = 3; // Layer du joueur
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    
    private void OnEnable() 
    {
		    PlayerRespawn.OnPlayerRespawn += ResetPlatform;
		}
		
		private void OnDisable()
		{
				PlayerRespawn.OnPlayerRespawn -= ResetPlatform;
		}
		
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision faite");
        if (other.gameObject.layer == playerLayer && _isTriggered == false)
        {
		        _isTriggered = true;
            StartCoroutine(FallSequence());
        }
    }
    
		private void ResetPlatform()
		{
				transform.position = _startPosition;
				transform.rotation= _startRotation;
				
				_rigidBody.isKinematic = true;
				_rigidBody.linearVelocity = Vector3.zero;
				_rigidBody.angularVelocity = Vector3.zero;
				
				_isTriggered = false;
		}
		
    private IEnumerator FallSequence()
    {
        yield return new WaitForSeconds(timeBeforeCollapse);
        _rigidBody.isKinematic = false;
    }
    
}