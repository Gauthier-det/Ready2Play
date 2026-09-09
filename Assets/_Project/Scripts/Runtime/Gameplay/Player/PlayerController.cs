using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float lethalVelocityThreshold = -10f;   

    [SerializeField] private float maxGrappleDistance = 40f;
    [SerializeField] private float maxGrappleSpeed = 35f;
    [SerializeField] private float climbSpeed = 2f;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float swingForce = 15f; 
    [SerializeField] private float drag = 0.5f; 
    [SerializeField] private float ropeShootSpeed = 80f; 

    private Vector3 _grappleVelocity; 
    private Vector3 _currentRopeEnd;

    private CharacterController _characterController;
    private PlayerInput _playerInput;
    private PlayerRespawn _playerRespawn;
    private Camera _mainCamera;
    private float _voidThreshold = -2f; 
    
    private float _verticalVelocity;
    private bool _canMove;

    private Animator _animator;


    private bool _isGrappling;
    private Vector3 _grapplePoint;
    private float _ropeLength;


    // Récupère les références aux composants et à la caméra principale.
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _playerRespawn = GetComponent<PlayerRespawn>();
        _mainCamera = Camera.main;

        _animator = GetComponentInChildren<Animator>();
    }

    // S'abonne aux changements d'état du jeu 
    private void Start()
    {
        GameManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(GameManager.Instance.CurrentState);
    }

    // Se désabonne pour éviter les fuites mémoire.
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }
    }

    // Boucle principale : lit l'input, oriente le perso et le déplace si autorisé.
    private void Update()
    {
        if (!_canMove) return;

        HandleGrappleInput();

        if(_isGrappling == true)
        {
            HandleGrappleMovement();
        }
        else
        {
            HandleNormalMovement();
        }
    }

    private void HandleGrappleInput()
    {
        if (_playerInput.actions["Grapple"].WasPressedThisFrame())
        {
            if (_isGrappling) 
            {
                _isGrappling = false;
                if (lineRenderer != null) lineRenderer.enabled = false;
                
                _verticalVelocity = _grappleVelocity.y + (jumpForce * 0.5f);
            }
            else
            {
                Vector3 rayOrigin = _mainCamera.transform.position;
                Vector3 rayDirection = _mainCamera.transform.forward;


                Debug.DrawRay(rayOrigin, rayDirection * maxGrappleDistance, Color.red, 2f);
                if (Physics.SphereCast(rayOrigin, 1.5f, rayDirection, out RaycastHit hit, maxGrappleDistance, grappleLayer))
                {
                    _isGrappling = true;
                    _grapplePoint = hit.point;
                    _ropeLength = Vector3.Distance(transform.position, _grapplePoint);

                    _currentRopeEnd = transform.position; // La corde part du joueur
                    _grappleVelocity = Vector3.zero; // On remet l'élan à zéro

                    if (lineRenderer != null) 
                    {
                        lineRenderer.enabled = true;
                    }
                }
            }
        }
    }

    private void HandleGrappleMovement()
    {
        Debug.Log("Bien appelé aussi");

        if (lineRenderer != null)
        {
            _currentRopeEnd = Vector3.MoveTowards(_currentRopeEnd, _grapplePoint, ropeShootSpeed * Time.deltaTime);

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, _grapplePoint);
        }

        if (_playerInput.actions["Jump"].IsPressed())
        {
            _ropeLength -= climbSpeed * Time.deltaTime;
        }
        else if (_playerInput.actions["Sprint"].IsPressed()) 
        {
            _ropeLength += climbSpeed * Time.deltaTime; 
        }

        if (_ropeLength < 1f) _ropeLength = 1f;
        if (_ropeLength > maxGrappleDistance) _ropeLength = maxGrappleDistance;

        Vector3 ropeDirection = (transform.position - _grapplePoint).normalized;
        float currentDistance = Vector3.Distance(transform.position, _grapplePoint);

        // 1. Ajouter la gravité
        _grappleVelocity += Vector3.down * gravity * Time.deltaTime; 

        // 2. Ajouter la force du joueur (ZQSD)
        Vector2 input = ReadMoveInput();
        Vector3 swingDirection = ComputeCameraRelativeDirection(input);
        _grappleVelocity += swingDirection * swingForce * Time.deltaTime; 

        // 3. Ajouter la friction (Résistance de l'air)
        _grappleVelocity -= _grappleVelocity * drag * Time.deltaTime; 

        // 4. La contrainte de la corde (Le Balancier)
        if (currentDistance >= _ropeLength)
        {
            // La magie des maths : Projeter la vélocité sur la tangente
            _grappleVelocity = Vector3.ProjectOnPlane(_grappleVelocity, ropeDirection);

            // Sécurité : Ramener le joueur s'il dépasse la longueur de la corde
            Vector3 idealPosition = _grapplePoint + ropeDirection * _ropeLength;
            Vector3 tensionCorrection = idealPosition - transform.position;
            _characterController.Move(tensionCorrection);
        }

        if (_grappleVelocity.magnitude > maxGrappleSpeed)
        {
            _grappleVelocity = _grappleVelocity.normalized * maxGrappleSpeed;
        }

        // 5. Appliquer le mouvement final
        _characterController.Move(_grappleVelocity * Time.deltaTime);

        if (swingDirection.sqrMagnitude > 0.1f) 
        {
            RotateTowards(swingDirection);
        }
        else if (_grappleVelocity.sqrMagnitude > 0.1f)
        {
            Vector3 lookDir = _grappleVelocity;
            lookDir.y = 0; // On garde le perso droit
            RotateTowards(lookDir.normalized);
        }
    }

    private void HandleNormalMovement() //Contient tout l'ancien update
    {
        Vector2 input = ReadMoveInput();
        Vector3 moveDirection = ComputeCameraRelativeDirection(input);

        RotateTowards(moveDirection);
        ApplyGravity();

        if (_playerInput.actions["Jump"].WasPressedThisFrame() && _characterController.isGrounded)
        {
            _verticalVelocity = jumpForce; 
        }

        CollisionFlags flags = _characterController.Move(moveDirection * walkSpeed * Time.deltaTime + Vector3.up * _verticalVelocity * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0)
        {
            _verticalVelocity = -2f; 
        }

        if ((flags & CollisionFlags.Below) != 0) // Si le joueur tombe de trop haut
        {
            if (_verticalVelocity < lethalVelocityThreshold)
            {
                Debug.Log("t mors");
                _playerRespawn.Respawn(PlayerRespawn.RespawnMode.Fall);
            }
        }

        if (transform.position.y < _voidThreshold && GameManager.Instance.CurrentState != GameState.Respawning) // Si le joueur tombe dans le vide
        {
            _verticalVelocity = 0f; 
            _playerRespawn.Respawn(PlayerRespawn.RespawnMode.Void);
        }

        if (_animator != null)
        {
            _animator.SetFloat("Speed", moveDirection.magnitude);
        }
    }

    // Autorise le mouvement uniquement en exploration ou en parcours.
    private void HandleStateChanged(GameState newState)
    {
        _canMove = (newState == GameState.FreeExploration || newState == GameState.InLevel);
    }

    // Lit la valeur de l'action Move de l'Input System.
    private Vector2 ReadMoveInput()
    {
        return _playerInput.actions["Move"].ReadValue<Vector2>();
    }

    // Convertit l'input en direction de déplacement relative à la caméra.
    private Vector3 ComputeCameraRelativeDirection(Vector2 input)
    {
        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        // Projeter l'entrée sur les axes forward et right de la caméra
        Vector3 moveDirection = forward * input.y + right * input.x;
        // Aplatis sur l'axe Y pour rester horizontal
        moveDirection.y = 0;
        // Normaliser pour éviter les vitesses variables
        if (moveDirection.sqrMagnitude > 0)
            moveDirection.Normalize();

        return moveDirection;
    }

    // Oriente progressivement le perso vers sa direction de déplacement.
    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Met à jour la vélocité verticale en fonction du contact au sol.
    private void ApplyGravity()
    {
        if (_characterController.isGrounded)
        {
            _verticalVelocity = -2f; // Petite force vers le bas pour rester au sol
        }
        else
        {
            _verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // On choisit la couleur jaune
        Gizmos.color = Color.yellow;
        // On dessine une sphère vide autour du joueur, de la taille de maxGrappleDistance
        Gizmos.DrawWireSphere(transform.position, maxGrappleDistance);
    } 
}