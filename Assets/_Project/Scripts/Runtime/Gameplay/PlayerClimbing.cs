using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class SimpleWallClimber : MonoBehaviour
{
    [Header("Dépendances")]
    [Tooltip("Glisse ici ton script de mouvement de base pour qu'il soit désactivé pendant l'escalade")]
    [SerializeField] private MonoBehaviour baseControllerScript;

    [Header("Paramètres d'escalade")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private float detectionDistance = 0.6f;

    private CharacterController _characterController;
    private PlayerInput _playerInput;
    
    private bool _isClimbing;
    private float _originalStepOffset;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        
        // On mémorise cette valeur car elle cause souvent des "mini sauts" indésirables contre les murs
        _originalStepOffset = _characterController.stepOffset;
    }

    private void Update()
    {
        Vector2 input = _playerInput.actions["Move"].ReadValue<Vector2>();

        // 1. Est-ce qu'on a un mur droit devant nous ?
        bool wallInFront = CheckWallInFront();

        // 2. Si on a un mur ET qu'on essaie d'avancer (input.y > 0)
        if (wallInFront && input.y > 0.1f)
        {
            if (!_isClimbing)
            {
                StartClimbing();
                Debug.Log("Tu commences à monter là");
            }
            Debug.Log("Tu montes là");
            
            // On déplace le personnage STRICTEMENT vers le haut
            _characterController.Move(Vector3.up * (climbSpeed * Time.deltaTime));
        }
        else
        {
            // Si on ne détecte plus de mur (on est arrivé en haut) OU qu'on arrête d'avancer
            if (_isClimbing)
            {
                // NOUVEAU : Si on arrête de détecter le mur mais qu'on avance toujours, on se hisse !
                if (input.y > 0.1f)
                {
                    VaultLedge();
                }

                StopClimbing();
                Debug.Log("T'arrêtes de monter là");
            }
        }
    }

    private bool CheckWallInFront()
    {
        // On tire un rayon depuis le torse du personnage vers l'avant
        Vector3 origin = transform.position + Vector3.up * (_characterController.height / 2f);
        
        // On ajoute une petite tolérance de distance une fois qu'on grimpe pour ne pas décrocher
        float currentDist = _isClimbing ? detectionDistance + 0.2f : detectionDistance;
        
        return Physics.Raycast(origin, transform.forward, out RaycastHit hit, currentDist, climbableLayer);
    }

    private void StartClimbing()
    {
        _isClimbing = true;
        
        // Empêche le CharacterController de tenter d'enjamber le mur de force (finis les mini-sauts !)
        _characterController.stepOffset = 0f; 

        if (baseControllerScript != null)
        {
            baseControllerScript.enabled = false;
        }
    }

    private void StopClimbing()
    {
        _isClimbing = false;
        
        // On restaure l'enjambement normal pour les escaliers au sol
        _characterController.stepOffset = _originalStepOffset;

        if (baseControllerScript != null)
        {
            baseControllerScript.enabled = true;
        }
    }

    private void VaultLedge()
    {
        // 1. On donne un coup de boost vers le haut pour que les pieds dépassent le rebord du mur
        float heightToClear = (_characterController.height / 2f) + 0.6f;
        _characterController.Move(Vector3.up * heightToClear);

        // 2. On pousse légèrement le personnage en avant pour le poser de manière sécurisée sur le toit
        _characterController.Move(transform.forward * 0.5f);
        
        Debug.Log("Hissage réussi !");
    }
}