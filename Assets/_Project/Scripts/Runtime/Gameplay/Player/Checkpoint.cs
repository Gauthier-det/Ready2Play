
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private int order;



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Checkpoint {order} triggered by {other.name}");
        if (other.CompareTag("Player"))
        {
            PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();
            
            if (order > playerRespawn.GetRespawnPointOrder())
            {
                playerRespawn.SetRespawnPoint(respawnPoint);
                playerRespawn.SetRespawnPointOrder(order);
            }
        }
    }
}