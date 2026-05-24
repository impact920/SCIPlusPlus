using UnityEngine;

public class InfiniteParallax : MonoBehaviour
{
    private Transform cam;

    private float spriteWidth;

    private Vector3 startPos;

    [Header("Parallax")]
    [Range(0f, 1f)]
    public float parallaxEffect = 0.5f;

    private void Start()
    {
        cam = Camera.main.transform;

        startPos = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        spriteWidth = sr.bounds.size.x;
    }

    private void Update()
    {
        // PARALLAX
        float distance = cam.position.x * parallaxEffect;

        transform.position = new Vector3(
            startPos.x + distance,
            transform.position.y,
            transform.position.z
        );

        // INFINITE LOOP
        float relativeCamPos = cam.position.x * (1 - parallaxEffect);

        // dla 5 segmentów
        if (relativeCamPos > startPos.x + spriteWidth * 2f)
        {
            startPos.x += spriteWidth * 5f;
        }
        else if (relativeCamPos < startPos.x - spriteWidth * 2f)
        {
            startPos.x -= spriteWidth * 5f;
        }
    }
}