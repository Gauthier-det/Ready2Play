using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
            if (_isClimbing)
            {
                if (input.y > 0.1f)
                {
                    // On lance le hissage fluide (qui appellera StopClimbing à la fin)
                    StartCoroutine(VaultLedgeRoutine());
                }
                else
                {
                    // Si le joueur a lâché Z, on lâche juste le mur normalement
                    StopClimbing();
                }
                
                // On passe isClimbing à false IMMÉDIATEMENT pour que le script arrête d'appeler l'escalade,
                // même si StopClimbing sera réellement géré par la Coroutine.
                _isClimbing = false; 
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

    private IEnumerator VaultLedgeRoutine()
    {
        // 1. On calcule la hauteur totale à franchir
        float heightToClear = (_characterController.height / 2f) + 0.6f;
    
        // 2. On définit en combien de temps (en secondes) le personnage doit se hisser
        float vaultDuration = 0.2f; 
        float timePassed = 0f;

        // 3. La boucle fluide : tant qu'on n'a pas atteint la durée, on monte un peu à chaque frame
        while (timePassed < vaultDuration)
        {
            // On calcule la petite portion de hauteur à monter pour cette frame précise
            float climbStep = (heightToClear / vaultDuration) * Time.deltaTime;
        
            _characterController.Move(Vector3.up * climbStep);
        
            timePassed += Time.deltaTime;
        
            // On dit à Unity : "Pause la fonction ici et reprends à la frame suivante"
            yield return null; 
        }

        // 4. Une fois arrivé en haut, on réactive le script de base
        // C'est SEULEMENT maintenant que la touche Z du joueur va le pousser en avant sur le toit
        StopClimbing(); 
    }
}