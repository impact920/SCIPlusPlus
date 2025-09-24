using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerAttacking : MonoBehaviour
{
    public Animator animator;
    public float healthAmount = 100f;
    public Image healthBar;
    [SerializeField] TextMeshProUGUI CurrentHealthText;
    public int maxHealth = 100;
    int currentHealth;

    public LayerMask enemyLayers;
    public Transform AttackPoint;
    public float attackRange = 0.5f;
    public float attackRate = 2f;
    public int PlayerAttackDamage = 40;

    // Nieœmiertelnoœæ
    private bool isImmortal = false;
    public float immortalDuration = 3f; // Czas trwania nieœmiertelnoœci

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamageEnemy(PlayerAttackDamage);
        }
    }

    public void TakeDamagePlayer(int Damage)
    {
        if (isImmortal) return; // Ignoruj obra¿enia, gdy gracz jest nieœmiertelny

        currentHealth -= Damage;

        animator.SetTrigger("Hurt");

        healthBar.fillAmount = currentHealth / 100f;
        CurrentHealthText.text = "" + currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void HealPlayer(int Heal)
    {
        int c = currentHealth;
        if ((c = c + Heal) > 100)
        {
            currentHealth = 100;
        }
        else
        {
            currentHealth += Heal;
        }

        healthBar.fillAmount = currentHealth / 100f;
        CurrentHealthText.text = "" + currentHealth;
    }

    public void ActivateImmortality()
    {
        if (!isImmortal)
        {
            StartCoroutine(ImmortalityCoroutine());
        }
    }

    private IEnumerator ImmortalityCoroutine()
    {
        isImmortal = true;

        yield return new WaitForSeconds(immortalDuration);

        isImmortal = false;
    }

    void Die()
    {
          Debug.Log("Player Died");

        animator.SetBool("Death", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        SceneManager.LoadSceneAsync(2);
    }

    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
            return;

        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }
}
