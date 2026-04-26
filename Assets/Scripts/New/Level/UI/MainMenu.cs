using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        string sceneToLoad = "Level"; // domyślna scena

        if (PlayerPrefs.HasKey("CheckpointScene"))
        {
            sceneToLoad = PlayerPrefs.GetString("CheckpointScene");
        }

        LoadingScreen.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}