using UnityEngine;

public class ShopZone : MonoBehaviour
{
    public GameObject shopUI;
    private bool playerInside = false;
    private bool shopOpen = false;

    void Update()
    {
        // jeśli gracz w strefie LUB sklep jest otwarty
        if ((playerInside || shopOpen) && Input.GetButtonDown("Interact"))
        {
            ToggleShop();
        }
    }

    void ToggleShop()
    {
        shopOpen = !shopOpen;
        shopUI.SetActive(shopOpen);
        Time.timeScale = shopOpen ? 0f : 1f;

        // Blokada pauzy pod ESC
        GameState.UIBlocking = shopOpen;

        // kursor
        Cursor.lockState = shopOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = shopOpen;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }
}
