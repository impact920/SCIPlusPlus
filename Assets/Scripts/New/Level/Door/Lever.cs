using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private string leverID;
    [SerializeField] private Animator animator;

    // 🔥 TERAZ WIELE DRZWI
    [SerializeField] private Door[] doors;

    private bool playerInRange = false;
    private bool used = false;

    private string saveKey;

    private void Awake()
    {
        saveKey = "LEVER_" + leverID;

        if (animator == null)
            animator = GetComponent<Animator>();

        used = PlayerPrefs.GetInt(saveKey, 0) == 1;

        // jeśli już użyta ustaw animację dźwigni
        if (used)
        {
            animator.SetBool("Used", true);
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
        // otwieranie WSZYSTKICH drzwi
        foreach (Door door in doors)
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }

        //  ustawienie stanu użycia
        animator.SetBool("Used", true);
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