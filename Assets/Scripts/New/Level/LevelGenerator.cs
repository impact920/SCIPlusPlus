using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Ustawienia generatora")]
    [Tooltip("Lista prefabów segmentów, które będą użyte w podanej kolejności.")]
    public GameObject[] segmentPrefabs;

    [Tooltip("Obiekt gracza, względem którego mierzona jest odległość.")]
    public Transform player;

    [Tooltip("Odległość, przy której segment zostanie wczytany.")]
    public float loadDistance = 20f;

    private GameObject[] spawnedSegments;

    void Start()
    {
        spawnedSegments = new GameObject[segmentPrefabs.Length];

        // Na starcie nic nie wczytujemy – wszystko wczyta się dynamicznie
    }

    void Update()
    {
        float currentX = 0f;

        for (int i = 0; i < segmentPrefabs.Length; i++)
        {
            GameObject prefab = segmentPrefabs[i];

            // Jeśli segment już istnieje, pobierz jego długość
            float segmentLength = prefab.GetComponent<Segment>() != null
                ? prefab.GetComponent<Segment>().segmentLength
                : 10f;

            Vector3 segmentPosition = new Vector3(currentX, 0, 0);

            if (spawnedSegments[i] == null)
            {
                float distanceToPlayer = Vector3.Distance(player.position, segmentPosition);

                // Jeśli gracz jest wystarczająco blisko, wczytaj segment
                if (distanceToPlayer <= loadDistance)
                {
                    spawnedSegments[i] = Instantiate(prefab, segmentPosition, Quaternion.identity);
                    spawnedSegments[i].GetComponent<Segment>().isLoaded = true;
                }
            }

            currentX += segmentLength;
        }
    }
}
