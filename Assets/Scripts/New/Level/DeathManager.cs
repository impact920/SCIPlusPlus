using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    public GameObject deathMenuUI;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        deathMenuUI.SetActive(false);
    }

    public void ShowDeathMenu()
    {
        deathMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RespawnPlayer()
    {
        Time.timeScale = 1f;
        deathMenuUI.SetActive(false);

        if (playerHealth != null)
            playerHealth.Revive();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }
}
