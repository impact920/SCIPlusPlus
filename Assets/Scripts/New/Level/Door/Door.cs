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

        //  odczyt stanu
        isOpen = PlayerPrefs.GetInt(saveKey, 0) == 1;

        ApplyState();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        ApplyState();
    }

    private void ApplyState()
    {
        // najważniejsze: sterowanie animacją boolem
        animator.SetBool("IsOpen", isOpen);

        // collider tylko gdy zamknięte
        if (boxCollider != null)
            boxCollider.enabled = !isOpen;
    }
}