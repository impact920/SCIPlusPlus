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
            Vector3 newPoint = collision.transform.position;

            respawn.UpdateCheckpoint(newPoint);

            // Zapis pozycji
            PlayerPrefs.SetFloat("CheckpointX", newPoint.x);
            PlayerPrefs.SetFloat("CheckpointY", newPoint.y);
            PlayerPrefs.SetFloat("CheckpointZ", newPoint.z);

            // Zapis sceny
            string currentScene = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("CheckpointScene", currentScene);

            PlayerPrefs.SetInt("HasCheckpoint", 1);
            PlayerPrefs.Save();

            Debug.Log($"Zapisano checkpoint: {newPoint} w scenie: {currentScene}");

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                PlayerPrefs.SetString("CheckpointScene", sceneToLoad);
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}