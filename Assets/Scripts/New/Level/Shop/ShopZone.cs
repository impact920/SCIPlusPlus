using UnityEngine;

public class ShopZone : MonoBehaviour
{
    public GameObject shopUI;
    public KeyCode interactKey = KeyCode.X;

    private bool playerInside = false;
    private bool shopOpen = false;

    void Update()
    {
        // jeśli gracz w strefie LUB sklep już otwarty
        if ((playerInside || shopOpen) && Input.GetKeyDown(interactKey))
        {
            ToggleShop();
        }
    }

    void ToggleShop()
{
    shopOpen = !shopOpen;
    shopUI.SetActive(shopOpen);

    // pauza / wznowienie gry
    Time.timeScale = shopOpen ? 0f : 1f;

    // OBSŁUGA KURSORA
    Cursor.lockState = shopOpen ? CursorLockMode.None : CursorLockMode.Locked;
    Cursor.visible = shopOpen;
}


    public void CloseShop()
    {
        shopOpen = false;
        shopUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
