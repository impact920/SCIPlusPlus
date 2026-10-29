using UnityEngine;

public class LoopSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;

    void Start()
    {
        audioSource.clip = clip;
        audioSource.loop = false; // false = nie zapętla
        audioSource.Play();
    }
}