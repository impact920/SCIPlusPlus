using UnityEngine;

public class MinimapToggle : MonoBehaviour
{
    public GameObject minimapUI;

    private bool isOpen = false;

    private void Start()
    {
        if (minimapUI != null)
        {
            minimapUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    private void ToggleMap()
    {
        isOpen = !isOpen;
        Time.timeScale = isOpen ? 0f : 1f;

        if (minimapUI != null)
        {
            minimapUI.SetActive(isOpen);
        }
    }
}