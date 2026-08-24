using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;
    private InputAction interactAction;
    private PlayerInput _playerInput;
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        interactAction = _playerInput.actions["Interact"];
        interactAction.Enable();
    }

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);
        IInteractable closest = null;
        float closestSqrDistance = float.MaxValue;
        IInteractable interactable = null;

        for (int i = 0; i < hitColliders.Length; i++) { // On cherche l'objet interactable le plus proche 
            interactable = hitColliders[i].GetComponent<IInteractable>();
            if (interactable == null) continue;

            float sqrDistance = (hitColliders[i].transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closest = interactable;
            }
        }

        if (closest == null && _currentInteractable != null) { // Si aucun objet n'est proche, on désélectionne l'objet interactable actuel
            _currentInteractable.SetHighlighted(false);
            _currentInteractable = null;
        } else {
            if (closest != null && closest != _currentInteractable) { // On change l'objet interactable actuel si on en a trouvé un plus proche
                    closest.SetHighlighted(true);
                    _currentInteractable?.SetHighlighted(false);
                    _currentInteractable = closest;
            } 
        }

        if (interactAction.WasPressedThisFrame() && closest != null) { // On interagit avec l'objet le plus proche si on appuie sur le bouton d'interaction
            Interact(closest);
        }
    }

    private void Interact(IInteractable interactable)
    {
        if (GameManager.Instance.CurrentState == GameState.FreeExploration || GameManager.Instance.CurrentState == GameState.Dialog) {
            Debug.Log("Attempting to interact with closest object.");
            interactable.Interact();
        }
    }
}