using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapColorSetter : MonoBehaviour
{
    public Tilemap tilemap;

    [Header("Kolor w HEX (np. #DBDBDB)")]
    public string hexColor = "#DBDBDB";

    void Start()
    {
        SetTilemapColor(hexColor);
    }

    public void SetTilemapColor(string hex)
    {
        Color color;

        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            tilemap.color = color;
        }
        else
        {
            Debug.LogError("Niepoprawny HEX: " + hex);
        }
    }
}