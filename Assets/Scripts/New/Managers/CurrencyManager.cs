using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    public int currency = 0;
    public TMP_Text currencyText;

    private const string CURRENCY_KEY = "PLAYER_CURRENCY";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadCurrency();
        UpdateUI();
    }

    public void AddCurrency(int amount)
{
    currency += amount;
    PlayerPrefs.SetInt("Currency", currency);

    UpdateUI();

    // odśwież sklep jeśli jest otwarty
    ShopUIManager shop = FindObjectOfType<ShopUIManager>();
    if (shop != null && shop.gameObject.activeInHierarchy)
        shop.UpdateUI();
}


    void UpdateUI()
    {
        if (currencyText != null)
            currencyText.text = "" + currency;
    }

    // ZAPIS
    void SaveCurrency()
    {
        PlayerPrefs.SetInt(CURRENCY_KEY, currency);
        PlayerPrefs.Save();
    }

    // ODCZYT
    void LoadCurrency()
    {
        currency = PlayerPrefs.GetInt(CURRENCY_KEY, 0);
    }
}
