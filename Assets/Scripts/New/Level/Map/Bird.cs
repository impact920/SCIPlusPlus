using UnityEngine;
using System;

public class Bird : MonoBehaviour
{
    public float speed = 5f;
    public float maxDistanceFromPlayer = 100f;

    private Vector3 moveDirection;
    private Transform player;

    public Action OnDestroyed;

    public void Initialize(Vector3 direction, Vector3 playerPosition)
    {
        moveDirection = direction.normalized;

        // znajdź kamerę jako "gracza"
        player = Camera.main.transform;

        // 🔥 FLIP TEKSTURY
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (moveDirection.x < 0)
                sr.flipX = true;   // leci w lewo → flip
            else
                sr.flipX = false;  // leci w prawo → normal
        }
    }

    void Update()
    {
        // ruch
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // dystans od gracza
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > maxDistanceFromPlayer)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}