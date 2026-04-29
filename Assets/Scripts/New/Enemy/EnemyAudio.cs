using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Auto AudioSource")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sounds (optional)")]
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip explosionSound;

    private void Awake()
    {
        // Auto tworzenie AudioSource jeśli brak
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Domyślne ustawienia
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D audio
    }

    // Animation Event: Spawn
    public void PlaySpawnSound()
    {
        if (spawnSound == null) return;
        audioSource.PlayOneShot(spawnSound);
    }

    // Animation Event: Attack
    public void PlayAttackSound()
    {
        if (attackSound == null) return;
        audioSource.PlayOneShot(attackSound);
    }

    // Animation Event: Walk
    public void PlayWalkSound()
    {
        if (walkSound == null) return;
        audioSource.PlayOneShot(walkSound);
    }

    // Animation Event: Explosion / Death
    public void PlayExplosionSound()
    {
        if (explosionSound == null) return;
        audioSource.PlayOneShot(explosionSound);
    }
}