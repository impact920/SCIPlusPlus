using UnityEngine;

public class PlayerMinimapIcon : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Rect mapy")]
    public RectTransform mapRect;

    [Header("Granice świata")]
    public Vector2 worldMin;
    public Vector2 worldMax;

    private RectTransform iconRect;

    private void Awake()
    {
        iconRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (player == null || mapRect == null)
            return;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        // 0-1
        float percentX = Mathf.InverseLerp(worldMin.x, worldMax.x, player.position.x);
        float percentY = Mathf.InverseLerp(worldMin.y, worldMax.y, player.position.y);

        // Pozycja w UI
        float x = percentX * mapRect.rect.width;
        float y = percentY * mapRect.rect.height;

        // Korekta środka
        x -= mapRect.rect.width * 0.5f;
        y -= mapRect.rect.height * 0.5f;

        iconRect.anchoredPosition = new Vector2(x, y);
    }
}