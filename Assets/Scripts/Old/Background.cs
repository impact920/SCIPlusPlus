using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    [Header("Camera")]
    public Transform cam;

    [Header("Parallax")]
    [Range(0f, 1f)]
    public float parallaxEffect = 0.5f;

    [Header("Teleport settings")]
    public float tileDistance = 20f; // stała odległość między tłami
    public int tileCount = 3;        // ilość kafli

    private Vector3 lastCamPos;

    void Start()
    {
        lastCamPos = cam.position;
    }

    void Update()
    {
        HandleParallax();
        HandleTileTeleport();
        lastCamPos = cam.position;
    }

    private void HandleParallax()
    {
        float deltaX = cam.position.x - lastCamPos.x;
        transform.position += new Vector3(deltaX * parallaxEffect, 0f, 0f);
    }

    private void HandleTileTeleport()
    {
        foreach (Transform tile in transform)
        {
            float camDistance = cam.position.x - tile.position.x;

            if (camDistance > tileDistance)
            {
                tile.position += Vector3.right * tileDistance * tileCount;
            }
            else if (camDistance < -tileDistance)
            {
                tile.position -= Vector3.right * tileDistance * tileCount;
            }
        }
    }
}
