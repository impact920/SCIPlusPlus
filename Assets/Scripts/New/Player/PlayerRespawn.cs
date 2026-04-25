using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public Vector3 respawnPoint;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (PlayerPrefs.GetInt("HasCheckpoint", 0) == 1)
        {
            string savedScene = PlayerPrefs.GetString("CheckpointScene");
            string currentScene = SceneManager.GetActiveScene().name;

            // Jeśli jesteśmy w złej scenie → przeładuj właściwą
            if (currentScene != savedScene)
            {
                Debug.Log($"Zmiana sceny z {currentScene} na {savedScene}");
                SceneManager.LoadScene(savedScene);
                return;
            }

            // Jeśli scena się zgadza → ustaw pozycję
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");

            respawnPoint = new Vector3(x, y, z);
            transform.position = respawnPoint;

            Debug.Log($"Wczytano checkpoint: {respawnPoint} w scenie: {savedScene}");
        }
        else
        {
            respawnPoint = transform.position;
            Debug.Log($"Brak checkpointu – start: {respawnPoint}");
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

        Debug.Log($"Respawn w: {respawnPoint}");
    }
}