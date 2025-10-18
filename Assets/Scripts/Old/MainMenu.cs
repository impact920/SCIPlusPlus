using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public Button backToMenuButton;
    public Button hiddenRoomButton;

    void Start()
    {
        // Przypisanie funkcji do przycisków
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
        backToMenuButton.onClick.AddListener(BackToMenu);
        hiddenRoomButton.onClick.AddListener(GoToHiddenRoom);
    }


    void StartGame()
    {

        SceneManager.LoadScene("Level");
    }

    void QuitGame()
    {
        // Zamyka aplikacjê (w edytorze Unity wy³¹cza grê)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }


    void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    void GoToHiddenRoom()
    {
        SceneManager.LoadScene("DevRoom");
    }
}
