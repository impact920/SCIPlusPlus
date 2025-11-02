using UnityEngine;
using System.Collections.Generic;

public class ObjectStreamerFixed : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    private Rigidbody2D playerRb;

    [Header("Streaming Settings")]
    public float activationDistance = 25f;

    [Header("Objects To Stream")]
    public List<GameObject> objectsToStream = new List<GameObject>();

    private class StreamObject
    {
        public GameObject obj;
        public Vector3 position;
        public bool active;
    }

    private List<StreamObject> streamList = new List<StreamObject>();
    private float activationDistanceSqr;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Brak przypisanego gracza w ObjectStreamer!");
            return;
        }

        playerRb = player.GetComponent<Rigidbody2D>();
        activationDistanceSqr = activationDistance * activationDistance;

        if (objectsToStream.Count == 0)
        {
            foreach (Transform child in transform)
                objectsToStream.Add(child.gameObject);
        }

        foreach (var go in objectsToStream)
        {
            if (go == null) continue;
            streamList.Add(new StreamObject
            {
                obj = go,
                position = go.transform.position,
                active = go.activeSelf
            });
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2 playerPos = playerRb ? playerRb.position : (Vector2)player.position;

        foreach (var item in streamList)
        {
            float distSqr = ((Vector2)item.position - playerPos).sqrMagnitude;
            bool shouldBeActive = distSqr < activationDistanceSqr;

            if (shouldBeActive != item.active)
            {
                item.obj.SetActive(shouldBeActive);
                item.active = shouldBeActive;
            }
        }
    }
}
