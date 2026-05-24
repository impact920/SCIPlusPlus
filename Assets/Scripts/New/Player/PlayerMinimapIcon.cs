using UnityEngine;

public class PlayerMinimapIcon : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Granice świata")]
    public Transform worldMinMarker;
    public Transform worldMaxMarker;

    [Header("Mapa UI")]
    public RectTransform mapRect;

    private RectTransform iconRect;

    private void Awake()
    {
        iconRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (player == null)
            return;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float percentX = Mathf.InverseLerp(
            worldMinMarker.position.x,
            worldMaxMarker.position.x,
            player.position.x
        );

        float percentY = Mathf.InverseLerp(
            worldMinMarker.position.y,
            worldMaxMarker.position.y,
            player.position.y
        );

        float mapX = (percentX * mapRect.rect.width) - (mapRect.rect.width / 2f);
        float mapY = (percentY * mapRect.rect.height) - (mapRect.rect.height / 2f);

        iconRect.anchoredPosition = new Vector2(mapX, mapY);
    }
}