using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SegmentConnection
{
    public GameObject segment; // Prefab segmentu
    public List<GameObject> validConnections; // Lista dozwolonych połączeń (jeśli pusta, łączy się ze wszystkimi)
}

public class LevelGenerator : MonoBehaviour
{
    public Transform player; // Referencja do gracza
    public List<GameObject> segmentPrefabs; // Lista prefabrykatów segmentów
    public float segmentSpawnDistance = 20f; // Odległość przed graczem, w której generowane są segmenty
    public int initialSegments = 5; // Liczba segmentów generowanych na początku
    public Transform startPoint; // Punkt początkowy generowania poziomu

    [Header("Konfiguracja segmentów")]
    public float segmentLength = 10f; // Długość segmentów (do ręcznego ustawienia w edytorze)

    private Queue<GameObject> activeSegments = new Queue<GameObject>(); // Kolejka aktywnych segmentów
    private float lastSpawnPosition; // Ostatnia pozycja, na której wygenerowano segment

    [Header("Zasady łączenia segmentów")]
    public List<SegmentConnection> segmentConnectionsList; // Lista konfiguracji połączeń segmentów

    private Dictionary<GameObject, List<GameObject>> segmentConnections = new Dictionary<GameObject, List<GameObject>>();
    private GameObject lastSegment;

    // Lista ostatnich segmentów (dla unikania powtórzeń)
    private List<GameObject> recentSegments = new List<GameObject>();
    public int recentSegmentLimit = 5;

    private void Start()
    {
        lastSpawnPosition = startPoint.position.x;

        // Konwertuj listę połączeń do słownika
        InitializeSegmentConnections();

        // Wstępna generacja segmentów
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnSegment();
        }
    }

    private void InitializeSegmentConnections()
    {
        segmentConnections.Clear();
        foreach (var connection in segmentConnectionsList)
        {
            if (connection.segment != null)
            {
                // Jeśli lista validConnections jest pusta, pozwól na połączenie z każdym segmentem
                if (connection.validConnections == null || connection.validConnections.Count == 0)
                {
                    segmentConnections[connection.segment] = new List<GameObject>(segmentPrefabs);
                }
                else
                {
                    segmentConnections[connection.segment] = connection.validConnections;
                }
            }
        }
    }

    private void Update()
    {
        // Sprawdzanie, czy należy wygenerować nowy segment
        if (player.position.x + segmentSpawnDistance > lastSpawnPosition)
        {
            SpawnSegment();
        }
    }

    private void SpawnSegment()
    {
        GameObject segmentPrefab = GetNextSegment();
        if (segmentPrefab == null)
        {
            Debug.LogError("Brak dostępnych połączeń dla segmentu: " + (lastSegment != null ? lastSegment.name : "start"));
            return;
        }

        GameObject newSegment = Instantiate(segmentPrefab);

        // Ustawienie pozycji segmentu na tej samej wysokości, ale z zachowaniem stałej odległości
        Vector3 spawnPosition = new Vector3(lastSpawnPosition, startPoint.position.y, 0);
        newSegment.transform.position = spawnPosition;

        // Zaktualizowanie pozycji ostatniego segmentu o stałą szerokość segmentu
        lastSpawnPosition += segmentLength; // Przesuwamy o stałą długość segmentu

        // Dodanie segmentu do kolejki i usunięcie starego segmentu
        activeSegments.Enqueue(newSegment);
        if (activeSegments.Count > initialSegments)
        {
            Destroy(activeSegments.Dequeue());
        }

        // Dodanie segmentu do listy ostatnich segmentów
        recentSegments.Add(segmentPrefab);
        if (recentSegments.Count > recentSegmentLimit)
        {
            recentSegments.RemoveAt(0); // Usuwamy najstarszy segment
        }

        // Ustawienie ostatniego segmentu
        lastSegment = segmentPrefab;
    }

    private GameObject GetNextSegment()
    {
        List<GameObject> validSegments = new List<GameObject>();

        if (lastSegment == null)
        {
            // Jeśli nie ma jeszcze ostatniego segmentu, wybierz losowy
            validSegments.AddRange(segmentPrefabs);
        }
        else if (segmentConnections.TryGetValue(lastSegment, out List<GameObject> validNextSegments))
        {
            validSegments.AddRange(validNextSegments);
        }

        // Usuń segmenty znajdujące się w ostatnich `recentSegmentLimit`
        validSegments.RemoveAll(segment => recentSegments.Contains(segment));

        if (validSegments.Count > 0)
        {
            return validSegments[Random.Range(0, validSegments.Count)];
        }

        // Jeśli wszystkie segmenty zostały wykluczone, wybierz losowy segment z pełnej listy (awaryjnie)
        return segmentPrefabs[Random.Range(0, segmentPrefabs.Count)];
    }
}
