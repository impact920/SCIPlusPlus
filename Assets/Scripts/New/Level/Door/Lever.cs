using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private string leverID;   // unikalne ID
    [SerializeField] private Animator animator;
    [SerializeField] private Door door;

    private bool playerInRange = false;
    private bool used = false;

    private string saveKey;

    private void Awake()
    {
        saveKey = "LEVER_" + leverID;

        if (animator == null)
            animator = GetComponent<Animator>();

        used = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (used)
        {
            // ustaw stan dźwigni po użyciu
            animator.Play("UsedState", 0, 1f); // opcjonalnie
        }
    }

    private void Update()
    {
        if (playerInRange && !used)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                ActivateLever();
            }
        }
    }

    private void ActivateLever()
    {
        used = true;

        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        animator.SetTrigger("Pull");
    }

    // Animation Event
    public void OnLeverPulled()
    {
        if (door != null)
        {
            door.OpenDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}