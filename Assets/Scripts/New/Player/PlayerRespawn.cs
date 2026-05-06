using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    public Vector3 respawnPoint;
    private Rigidbody2D rb;

    void Start()
    {
        StartCoroutine(LoadCheckpoint());
    }

    IEnumerator LoadCheckpoint()
    {
        // czekamy aż scena się ustabilizuje
        yield return null;

        rb = GetComponent<Rigidbody2D>();

        if (PlayerPrefs.GetInt("HasCheckpoint", 0) == 1)
        {
            string savedScene = PlayerPrefs.GetString("CheckpointScene");
            string currentScene = SceneManager.GetActiveScene().name;

            // jeśli zła scena → przeładuj
            if (currentScene != savedScene)
            {
                SceneManager.LoadScene(savedScene);
                yield break;
            }

            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");

            respawnPoint = new Vector3(x, y, z);
            transform.position = respawnPoint;

            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            respawnPoint = transform.position;
        }

        // 🔥 po ustawieniu gracza ustaw kamerę
        FindFirstObjectByType<CameraFollow>()?.SnapToTarget();
    }

    public void UpdateCheckpoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }

    public void Respawn()
    {
        transform.position = respawnPoint;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}