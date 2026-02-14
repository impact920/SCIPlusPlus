using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button healButton;
    public TMP_Text healText;

    public Button damageButton;
    public TMP_Text damageText;

    public Button speedButton;
    public TMP_Text speedText;

    public Button healthButton;
    public TMP_Text healthText;

    [Header("Costs")]
    public int baseHealCost = 5;
    public int baseUpgradeCost = 10;
    public int costIncreasePerLevel = 5; // cena rośnie liniowo z poziomem

    [Header("References")]
    public PlayerHealth playerHealth;
    public PlayerStats playerStats;

    void OnEnable()
    {
        UpdateUI();
    }

    // 🔹 Funkcja do aktualizacji wszystkich przycisków
    public void UpdateUI()
    {
        // Heal
        int healCost = baseHealCost;
        healText.text = $"Heal +20 HP - Cost: {healCost}";

        // nie kupisz, jeśli pełne życie
            healButton.interactable = 
            CurrencyManager.instance.currency >= healCost && 
            playerHealth.currentHealth < playerHealth.maxHealth;


        // Damage
        int dmgLevel = UpgradeManager.instance.damageLevel;
        int dmgCost = baseUpgradeCost + dmgLevel * costIncreasePerLevel;
        damageText.text = $"Damage Lv.{dmgLevel}/10 - Cost: {dmgCost}";
        damageButton.interactable = dmgLevel < 10 && CurrencyManager.instance.currency >= dmgCost;

        // Speed
        int spdLevel = UpgradeManager.instance.speedLevel;
        int spdCost = baseUpgradeCost + spdLevel * costIncreasePerLevel;
        speedText.text = $"Speed Lv.{spdLevel}/10 - Cost: {spdCost}";
        speedButton.interactable = spdLevel < 10 && CurrencyManager.instance.currency >= spdCost;

        // Max Health
        int hpLevel = UpgradeManager.instance.healthLevel;
        int hpCost = baseUpgradeCost + hpLevel * costIncreasePerLevel;
        healthText.text = $"Max Health Lv.{hpLevel}/10 - Cost: {hpCost}";
        healthButton.interactable = hpLevel < 10 && CurrencyManager.instance.currency >= hpCost;
    }

    // 🔹 Kupowanie
    public void BuyHeal()
{
    int cost = baseHealCost;

    // sprawdzamy czy gracz ma mniej niż max HP i wystarczającą walutę
    if (CurrencyManager.instance.currency < cost || playerHealth.currentHealth >= playerHealth.maxHealth)
        return;

    CurrencyManager.instance.AddCurrency(-cost);
    playerHealth.Heal(20);
    UpdateUI();
}


    public void BuyDamage()
    {
        int level = UpgradeManager.instance.damageLevel;
        if (level >= 10) return;

        int cost = baseUpgradeCost + level * costIncreasePerLevel;
        if (CurrencyManager.instance.currency < cost) return;

        CurrencyManager.instance.AddCurrency(-cost);
        UpgradeManager.instance.UpgradeDamage();
        playerStats.ApplyUpgrades();
        UpdateUI();
    }

    public void BuySpeed()
    {
        int level = UpgradeManager.instance.speedLevel;
        if (level >= 10) return;

        int cost = baseUpgradeCost + level * costIncreasePerLevel;
        if (CurrencyManager.instance.currency < cost) return;

        CurrencyManager.instance.AddCurrency(-cost);
        UpgradeManager.instance.UpgradeSpeed();
        playerStats.ApplyUpgrades();
        UpdateUI();
    }

    public void BuyHealthUpgrade()
    {
        int level = UpgradeManager.instance.healthLevel;
        if (level >= 10) return;

        int cost = baseUpgradeCost + level * costIncreasePerLevel;
        if (CurrencyManager.instance.currency < cost) return;

        CurrencyManager.instance.AddCurrency(-cost);
        UpgradeManager.instance.UpgradeHealth();
        playerStats.ApplyUpgrades();
        UpdateUI();
    }
}
