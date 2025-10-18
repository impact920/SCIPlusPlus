using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Collecting : MonoBehaviour
{

    int Coins = 0;
    public int Heal = 40;
    private PlayerAttacking playerAttacking;
    [SerializeField] TextMeshProUGUI CoinsText;

    void Start()
    {

        playerAttacking = GetComponent<PlayerAttacking>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            Coins++;

            CoinsText.text = " : " + Coins;
        }

        if (other.gameObject.CompareTag("Heal"))
        {
            Destroy(other.gameObject);

            playerAttacking.HealPlayer(Heal);
        }
    }

}
