using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CurrencyManager.instance.AddCurrency(value);

            // niszczymy CAŁĄ monetę (czyli rodzica)
            Destroy(transform.root.gameObject);
        }
    }
}
