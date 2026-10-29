using UnityEngine;
using UnityEngine.EventSystems;

public class SelectFirstButton : MonoBehaviour
{
    public GameObject firstButton;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}