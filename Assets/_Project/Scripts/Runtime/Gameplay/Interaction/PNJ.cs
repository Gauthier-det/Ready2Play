using UnityEngine;

public class PNJ : MonoBehaviour, IInteractable {
    [SerializeField] private string npcId;
    [SerializeField] private int order = 1;
    [SerializeField] private DialogContainer dialogContainer;
    [SerializeField] private bool isHighlighted;
    [SerializeField] private Color highlightColor = Color.yellow;

    private MeshRenderer _meshRenderer;
    private Color _originalColor;

    private void Awake(){
        _meshRenderer = GetComponent<MeshRenderer>();
        _originalColor = _meshRenderer.material.color;
    }

    public void Interact() {
        bool previousOrderDone = order <= 1 || NpcMemoryService.Instance.HasVisitedOrder(npcId, order - 1);
        bool alreadyVisited = previousOrderDone && NpcMemoryService.Instance.HasVisitedOrder(npcId, order);

        if (previousOrderDone) {
            NpcMemoryService.Instance.MarkOrderVisited(npcId, order);
            DestroyEarlierVisitedVersions();
        }

        DialogRunner.Instance.StartDialog(npcId, dialogContainer, alreadyVisited);
    }

    private void DestroyEarlierVisitedVersions() {
        PNJ[] allPnjs = FindObjectsByType<PNJ>(FindObjectsSortMode.None);
        foreach (PNJ other in allPnjs) {
            if (other == this) continue;
            if (other.npcId != npcId) continue;
            if (other.order >= order) continue;
            if (!NpcMemoryService.Instance.HasVisitedOrder(npcId, other.order)) continue;
            Destroy(other.gameObject);
        }
    }

    public void SetHighlighted(bool highlighted) {
        isHighlighted = highlighted;
        _meshRenderer.material.color = highlighted ? highlightColor : _originalColor;
    }
}