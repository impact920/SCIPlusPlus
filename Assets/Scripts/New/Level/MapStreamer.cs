using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[ExecuteAlways]
public class AutoTilemapChunkerFinal : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;              // Obiekt gracza
    private Rigidbody2D playerRb;

    [Header("Chunk Settings")]
    public int chunkWidth = 16;           // Szerokość chunku w tile'ach
    public int chunkHeight = 16;          // Wysokość chunku w tile'ach
    public float activationDistance = 20f; // Zasięg aktywacji

    [Header("Chunk Options")]
    public bool addColliders = true;      // Czy dodawać kolizje
    public bool showGizmos = false;       // Debug Gizmos

    private Tilemap originalTilemap;
    private List<Chunk> chunks = new List<Chunk>();
    private float activationDistanceSqr;

    private class Chunk
    {
        public GameObject obj;
        public TilemapRenderer renderer;
        public TilemapCollider2D collider;
        public Vector3 center;
        public bool active;
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("⚠️ Brak przypisanego gracza — Tilemap będzie zawsze aktywna.");
        }
        else
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        activationDistanceSqr = activationDistance * activationDistance;

        originalTilemap = GetComponent<Tilemap>();
        if (originalTilemap == null)
        {
            Debug.LogError("❌ Brak komponentu Tilemap na tym obiekcie!");
            return;
        }

        // Tworzymy chunki tylko w trybie gry
        if (Application.isPlaying)
        {
            GenerateChunks();
            originalTilemap.ClearAllTiles();
        }
    }

    void GenerateChunks()
    {
        chunks.Clear();

        BoundsInt bounds = originalTilemap.cellBounds;

        for (int cx = bounds.xMin; cx < bounds.xMax; cx += chunkWidth)
        {
            for (int cy = bounds.yMin; cy < bounds.yMax; cy += chunkHeight)
            {
                CreateChunk(cx, cy,
                    Mathf.Min(chunkWidth, bounds.xMax - cx),
                    Mathf.Min(chunkHeight, bounds.yMax - cy));
            }
        }
    }

    void CreateChunk(int startX, int startY, int width, int height)
    {
        GameObject chunkObj = new GameObject($"Chunk_{startX}_{startY}");
        chunkObj.transform.parent = transform;
        chunkObj.transform.position = transform.position;
        chunkObj.transform.localScale = transform.localScale;

        // kopiujemy layer i tag
        chunkObj.layer = gameObject.layer;
        chunkObj.tag = gameObject.tag;

        Tilemap tilemap = chunkObj.AddComponent<Tilemap>();
        TilemapRenderer renderer = chunkObj.AddComponent<TilemapRenderer>();

        // kopiujemy ustawienia renderera z oryginalnej tilemapy
        TilemapRenderer originalRenderer = originalTilemap.GetComponent<TilemapRenderer>();
        if (originalRenderer != null)
        {
            renderer.sortingLayerID = originalRenderer.sortingLayerID;
            renderer.sortingLayerName = originalRenderer.sortingLayerName;
            renderer.sortingOrder = originalRenderer.sortingOrder;
            renderer.material = originalRenderer.sharedMaterial;
            renderer.mode = originalRenderer.mode;
            renderer.maskInteraction = originalRenderer.maskInteraction;
        }

        TilemapCollider2D collider = null;
        if (addColliders)
        {
            collider = chunkObj.AddComponent<TilemapCollider2D>();
            Rigidbody2D rb = chunkObj.AddComponent<Rigidbody2D>();
            CompositeCollider2D comp = chunkObj.AddComponent<CompositeCollider2D>();

            rb.bodyType = RigidbodyType2D.Static;
            collider.usedByComposite = true;
            comp.geometryType = CompositeCollider2D.GeometryType.Polygons;
            comp.generationType = CompositeCollider2D.GenerationType.Synchronous;
        }

        // kopiujemy tile'e do chunku
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(startX + x, startY + y, 0);
                TileBase tile = originalTilemap.GetTile(pos);
                if (tile != null)
                    tilemap.SetTile(pos, tile);
            }
        }

        renderer.enabled = false;
        if (collider != null) collider.enabled = false;

        // 🔧 dokładne liczenie środka w świecie (uwzględnia Grid i skalę)
        Grid grid = originalTilemap.layoutGrid;
        Vector3 worldMin = grid.CellToWorld(new Vector3Int(startX, startY, 0));
        Vector3 worldMax = grid.CellToWorld(new Vector3Int(startX + width, startY + height, 0));
        Vector3 center = (worldMin + worldMax) / 2f;

        chunks.Add(new Chunk
        {
            obj = chunkObj,
            renderer = renderer,
            collider = collider,
            center = center,
            active = false
        });
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        if (player == null) return;

        Vector2 playerPos = playerRb ? playerRb.position : (Vector2)player.position;

        foreach (var chunk in chunks)
        {
            float distSqr = ((Vector2)chunk.center - playerPos).sqrMagnitude;
            bool shouldBeActive = distSqr < activationDistanceSqr;

            if (shouldBeActive != chunk.active)
            {
                chunk.renderer.enabled = shouldBeActive;
                if (chunk.collider != null) chunk.collider.enabled = shouldBeActive;
                chunk.active = shouldBeActive;
            }
        }
    }

    // 🔍 Pomocnicze Gizmosy (opcjonalnie)
    void OnDrawGizmosSelected()
    {
        if (!showGizmos || chunks == null) return;
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        foreach (var chunk in chunks)
        {
            if (chunk != null)
                Gizmos.DrawWireSphere(chunk.center, activationDistance);
        }
    }
}
