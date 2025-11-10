using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerRespawn respawn = collision.GetComponent<PlayerRespawn>();
        if (respawn != null)
            respawn.UpdateCheckpoint(transform.position);
    }
}
