using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Vector3 respawnPoint;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 💾 Odczytaj checkpoint z pamięci
        if (PlayerPrefs.GetInt("HasCheckpoint", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");
            respawnPoint = new Vector3(x, y, z);

            Debug.Log($"Wczytano checkpoint z pamięci: {respawnPoint}");
            transform.position = respawnPoint;
        }
        else
        {
            respawnPoint = transform.position;
            Debug.Log($"Brak zapisanego checkpointu – ustawiono startowy: {respawnPoint}");
        }
    }

    public void UpdateCheckpoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log($"Checkpoint ustawiony na: {respawnPoint}");
    }

    public void Respawn()
    {
        transform.position = respawnPoint;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Debug.Log($"Gracz odrodzony w: {respawnPoint}");
    }
}
