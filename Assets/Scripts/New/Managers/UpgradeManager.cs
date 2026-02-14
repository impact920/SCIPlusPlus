using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public int damageLevel;
    public int speedLevel;
    public int healthLevel;

    private const string DAMAGE_KEY = "UPG_DAMAGE";
    private const string SPEED_KEY = "UPG_SPEED";
    private const string HEALTH_KEY = "UPG_HEALTH";

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        LoadUpgrades();
    }

    void Save()
    {
        PlayerPrefs.SetInt(DAMAGE_KEY, damageLevel);
        PlayerPrefs.SetInt(SPEED_KEY, speedLevel);
        PlayerPrefs.SetInt(HEALTH_KEY, healthLevel);
        PlayerPrefs.Save();
    }

    void LoadUpgrades()
    {
        damageLevel = PlayerPrefs.GetInt(DAMAGE_KEY, 0);
        speedLevel = PlayerPrefs.GetInt(SPEED_KEY, 0);
        healthLevel = PlayerPrefs.GetInt(HEALTH_KEY, 0);
    }

    public void UpgradeDamage()
    {
        if (damageLevel >= 10) return;
        damageLevel++;
        Save();
    }

    public void UpgradeSpeed()
    {
        if (speedLevel >= 10) return;
        speedLevel++;
        Save();
    }

    public void UpgradeHealth()
    {
        if (healthLevel >= 10) return;
        healthLevel++;
        Save();
    }
}
