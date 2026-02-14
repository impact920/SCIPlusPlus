using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int baseDamage = 20;       // Twoja wartość z PlayerHealth
    public float baseSpeed = 8f;      // moveSpeed z PlayerMovement
    public int baseMaxHealth = 100;   // maxHealth z PlayerHealth

    [HideInInspector] public int damage;
    [HideInInspector] public float speed;
    [HideInInspector] public int maxHealth;

    void Start()
    {
        ApplyUpgrades();
    }

    public void ApplyUpgrades()
    {
        damage = baseDamage + UpgradeManager.instance.damageLevel * 2;
        speed = baseSpeed + UpgradeManager.instance.speedLevel * 0.5f;
        maxHealth = baseMaxHealth + UpgradeManager.instance.healthLevel * 10;

        // Aktualizacja statystyk w skryptach gracza
        PlayerMovement pm = GetComponent<PlayerMovement>();
        PlayerHealth ph = GetComponent<PlayerHealth>();

        if (pm != null) pm.moveSpeed = speed;
        if (ph != null) ph.maxHealth = maxHealth;
    }
}
