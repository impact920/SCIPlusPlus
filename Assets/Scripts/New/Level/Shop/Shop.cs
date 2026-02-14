using UnityEngine;

public class Shop : MonoBehaviour
{
    public int healthCost = 5;
    public int healthAmount = 20;

    public PlayerHealth playerHealth; // referencja do życia gracza

    public void BuyHealth()
    {
        if (CurrencyManager.instance.currency >= healthCost)
        {
            CurrencyManager.instance.AddCurrency(-healthCost);
            playerHealth.Heal(healthAmount);
        }
        else
        {
            Debug.Log("Za mało monet!");
        }
    }
}
