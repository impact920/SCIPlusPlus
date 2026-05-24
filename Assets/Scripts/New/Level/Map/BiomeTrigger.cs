using UnityEngine;

public class BiomeTrigger : MonoBehaviour
{
    public Color biomeColor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            BackgroundColorManager.instance.ChangeColor(biomeColor);
        }
    }
}