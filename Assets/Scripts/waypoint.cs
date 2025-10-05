using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public Transform pointA;  // Obiekt reprezentuj¹cy pocz¹tkow¹ pozycjê
    public Transform pointB;  // Obiekt reprezentuj¹cy koñcow¹ pozycjê
    public float speed = 2f;  // Prêdkoœæ poruszania siê platformy
    public bool isLooping = true;  // Okreœla, czy platforma ma wracaæ do punktu pocz¹tkowego (true = w kó³ko)

    private bool movingToB = true;  // Kierunek ruchu (true = do punktu B, false = do punktu A)

    void Start()
    {
        // Ustawienie pocz¹tkowej pozycji platformy na pointA
        if (pointA != null)
        {
            transform.position = pointA.position;
        }
    }

    void Update()
    {
        // Jeœli platforma jest w trybie pêtli, przesuwamy j¹ miêdzy punktami A i B
        if (isLooping)
        {
            MovePlatform();
        }
        else
        {
            // Jeœli nie, platforma porusza siê tylko w jednym kierunku
            if (movingToB)
                MovePlatform();
        }
    }

    private void MovePlatform()
    {
        // Obliczenie kierunku ruchu
        Vector3 targetPosition = movingToB ? pointB.position : pointA.position;

        // Przemieszczamy platformê w stronê docelowego punktu
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Sprawdzamy, czy platforma osi¹gnê³a cel
        if (transform.position == targetPosition)
        {
            // Zmieniamy kierunek ruchu, jeœli platforma osi¹gnê³a cel
            movingToB = !movingToB;
        }
    }
}
