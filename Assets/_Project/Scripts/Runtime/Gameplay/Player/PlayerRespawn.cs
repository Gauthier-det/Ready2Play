using UnityEngine;
using System;


public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 0.5f;

    private int _respawnPointOrder = 0;

    private CharacterController _characterController;
    private bool _isRespawning;

    public static Action OnPlayerRespawn;

    public enum RespawnMode
    {
        Fall,
        Void
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void Respawn(RespawnMode mode)
    {
        if (_isRespawning) return;

        OnPlayerRespawn?.Invoke();

        _isRespawning = true;
        GameManager.Instance.SetState(GameState.Respawning);
        _characterController.enabled = false;
        if (mode == RespawnMode.Fall)
        {
            Invoke(nameof(FinishRespawn), respawnDelay);
        }
        else if (mode == RespawnMode.Void)
        {
            FinishRespawn();
        }
    }

    private void FinishRespawn()
    {
        transform.position = respawnPoint.position;
        _characterController.enabled = true;
        GameManager.Instance.SetState(GameState.FreeExploration);
        _isRespawning = false;
    }

    public int GetRespawnPointOrder()
    {
        return _respawnPointOrder;
    }

    public void SetRespawnPointOrder(int order)
    {
        _respawnPointOrder = order;
    }

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
}
