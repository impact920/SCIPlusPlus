using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerRespawn respawn = collision.GetComponent<PlayerRespawn>();
        if (respawn != null)
        {
            // TERAZ zapisujesz pozycję gracza
            Vector3 newPoint = collision.transform.position;

            respawn.UpdateCheckpoint(newPoint);

            PlayerPrefs.SetFloat("CheckpointX", newPoint.x);
            PlayerPrefs.SetFloat("CheckpointY", newPoint.y);
            PlayerPrefs.SetFloat("CheckpointZ", newPoint.z);
            PlayerPrefs.SetInt("HasCheckpoint", 1);
            PlayerPrefs.Save();

            Debug.Log($"Zapisano checkpoint: {newPoint}");

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}