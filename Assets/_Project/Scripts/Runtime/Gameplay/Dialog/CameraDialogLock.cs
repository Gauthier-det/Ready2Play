using UnityEngine;
using Unity.Cinemachine;

public class CameraDialogLock : MonoBehaviour
{
    private CinemachineInputAxisController _inputAxisController;

    private void Awake()
    {
        _inputAxisController = GetComponent<CinemachineInputAxisController>();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(GameManager.Instance.CurrentState);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }
    }

    private void HandleStateChanged(GameState newState)
    {
        _inputAxisController.enabled = newState != GameState.Dialog;
    }
}