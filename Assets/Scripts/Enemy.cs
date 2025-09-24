using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public int maxHealth = 100;
    int currentHealth;

    public float startingY;
    public Transform AttackPoint;
    public LayerMask playerLayers;
    public Transform player;

    public float attackRate = 1f;
    float nextAttackTime = 0f;

    public float detectionRadius = 8f;
    public float attackRange = 1f;
    public float moveSpeed = 5f;

    private Vector3 originalPosition;
    private bool isPlayerDetected = false;

    public int EnemyAttackDamage = 20;

    void Start()
    {
       currentHealth = maxHealth;
       originalPosition = transform.position;

    }

    void FixedUpdate()
    {
        Vector3 position = transform.position;
        position.y = startingY; // Sta³a wartoœæ osi Y
        transform.position = position;




        Vector3 targetPosition = isPlayerDetected ? player.position : originalPosition;
        MoveToPosition(targetPosition);
        float inputX = Input.GetAxis("Horizontal");

        if(transform.position == originalPosition)
            {
            animator.SetInteger("AnimState", 0);
            }

        if (Time.time >= nextAttackTime)
            {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, playerLayers);

            foreach(Collider2D Player in hitPlayers)
                {
                animator.SetTrigger("Attack");
                Player.GetComponent<PlayerAttacking>().TakeDamagePlayer(EnemyAttackDamage);
                nextAttackTime = Time.time + 1f / attackRate;
                }
            }


    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == player)
            {
            animator.SetInteger("AnimState", 2);
            GetComponent<SpriteRenderer>().flipX = false;
            isPlayerDetected = true;
            }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == player)
            {

            animator.SetInteger("AnimState", 2);
            GetComponent<SpriteRenderer>().flipX = true;
            isPlayerDetected = false;
            }
    }



    private void MoveToPosition(Vector3 targetPosition)
    {

    // Calculate direction to the target position

    Vector3 direction = (targetPosition - transform.position).normalized;
    // Move towards the target position

    float step = moveSpeed * Time.deltaTime; // Calculate the distance to move

    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }



    private void OnDrawGizmos()
    {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if(AttackPoint == null)
            return;

        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }




    public void TakeDamageEnemy(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        if(currentHealth <=0 )
        {
        Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died");

        animator.SetBool("Death", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

    }
}




