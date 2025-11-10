using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Vector3 respawnPoint;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position; // startowa pozycja
        Debug.Log("Startowy checkpoint: " + respawnPoint);
    }

    public void UpdateCheckpoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log("Checkpoint ustawiony na: " + respawnPoint);
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        Debug.Log("Gracz odrodzony w: " + respawnPoint);
    }
}
