using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        LoadingScreen.LoadScene("Level"); // nazwa sceny z rozgrywką
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
