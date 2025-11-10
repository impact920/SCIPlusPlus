using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerRespawn respawn = collision.GetComponent<PlayerRespawn>();
        if (respawn != null)
        {
            Vector3 newPoint = transform.position;
            respawn.UpdateCheckpoint(newPoint);

            // 💾 Zapisujemy w PlayerPrefs
            PlayerPrefs.SetFloat("CheckpointX", newPoint.x);
            PlayerPrefs.SetFloat("CheckpointY", newPoint.y);
            PlayerPrefs.SetFloat("CheckpointZ", newPoint.z);
            PlayerPrefs.SetInt("HasCheckpoint", 1);
            PlayerPrefs.Save();

            Debug.Log($"Zapisano checkpoint w pamięci: {newPoint}");
        }
    }
}
