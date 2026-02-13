using UnityEngine;
using TMPro; // konieczne dla TextMeshPro

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;
    public int currency = 0;          // aktualna waluta
    public TMP_Text currencyText;     // TextMeshPro UI

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        currencyText.text = "Coins: " + currency;
    }
}
