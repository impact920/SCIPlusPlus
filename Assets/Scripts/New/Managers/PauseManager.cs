using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;

    bool isPaused = false;

    void Update()
    {
        // jeśli jakieś UI blokuje grę (np. sklep) nie reaguj na ESC
        if (GameState.UIBlocking)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
{
    pauseMenu.SetActive(true);
    Time.timeScale = 0f;
    isPaused = true;

    // pokaż kursor
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

public void ResumeGame()
{
    pauseMenu.SetActive(false);
    Time.timeScale = 1f;
    isPaused = false;

    // schowaj kursor
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}


    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }
}
