using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private string doorID;
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D boxCollider;

    private bool isOpen = false;
    private string saveKey;

    private void Awake()
    {
        saveKey = "DOOR_" + doorID;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        isOpen = PlayerPrefs.GetInt(saveKey, 0) == 1;

        ApplyStateInstant();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        // OD RAZU animacja
        animator.SetBool("IsOpen", true);

        if (boxCollider != null)
            boxCollider.enabled = false;
    }

    private void ApplyStateInstant()
    {
        animator.SetBool("IsOpen", isOpen);

        if (boxCollider != null)
            boxCollider.enabled = !isOpen;
    }
}