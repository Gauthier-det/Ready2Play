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
    [SerializeField] private float lethalVelocityThreshold = -6f; 

    private CharacterController _characterController;
    private PlayerInput _playerInput;
    private PlayerRespawn _playerRespawn;
    private Camera _mainCamera;
    private float _voidThreshold = -2f; 
    
    private float _verticalVelocity;
    private bool _canMove;

    private Animator _animator;


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
}