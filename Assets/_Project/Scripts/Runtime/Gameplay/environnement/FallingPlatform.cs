using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float timeBeforeCollapse = 3f;
    [SerializeField] private float magnitude = 0.0001f;


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
        //yield return new WaitForSeconds(timeBeforeCollapse);
        float elasped = 0.0f;
        while (elasped < timeBeforeCollapse)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(_startPosition.x + x, _startPosition.y + y, _startPosition.z);
            elasped += Time.deltaTime;
            yield return null;
        }

        transform.position = _startPosition;
        _rigidBody.isKinematic = false;
    }
    
}