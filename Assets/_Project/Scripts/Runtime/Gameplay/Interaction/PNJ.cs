using UnityEngine;

public class PNJ : MonoBehaviour, IInteractable {
    [SerializeField] private string dialogText;
    [SerializeField] private bool isHighlighted;
    [SerializeField] private Color highlightColor = Color.yellow;

    private MeshRenderer _meshRenderer;
    private Color _originalColor;

    private void Awake(){
        _meshRenderer = GetComponent<MeshRenderer>();
        _originalColor = _meshRenderer.material.color;
    }

    public void Interact() {
        Debug.Log($"Interacting with PNJ: {dialogText}");
        GameManager.Instance.SetState(GameState.Dialog);
        Debug.Log("Bah oilà t'es coincé dans le mode dialog mon con");
    }

    public void SetHighlighted(bool highlighted) {
        isHighlighted = highlighted;
        _meshRenderer.material.color = highlighted ? highlightColor : _originalColor;
    }
}