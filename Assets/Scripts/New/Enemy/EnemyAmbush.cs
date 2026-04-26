using UnityEngine;

public class EnemyAmbush : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 3f;

    [Header("Hitbox")]
    public GameObject damageHitbox;

    private Animator animator;
    private bool isHidden = true;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // na start hitbox wyłączony
        if (damageHitbox != null)
            damageHitbox.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange && isHidden && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        isHidden = false;
        isAttacking = true;

        animator.SetTrigger("Attack");
    }

    //  Animation Event: moment aktywacji hitboxa (np. środek wyskoku)
    public void EnableHitbox()
    {
        if (damageHitbox != null)
            damageHitbox.SetActive(true);
    }

    //  Animation Event: koniec ataku
    public void Hide()
    {
        if (damageHitbox != null)
            damageHitbox.SetActive(false);

        isHidden = true;
        isAttacking = false;
    }
}