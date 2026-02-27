using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;                // ważne! dodaj to dla TextMeshPro
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static string sceneToLoad;   // scena, którą chcemy załadować
    public Slider progressBar;          // opcjonalny pasek postępu (może być null)
    public TMP_Text progressText;       // TextMeshPro zamiast zwykłego Text

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    public static void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        SceneManager.LoadScene("LoadingScene"); // przełącz na ekran ładowania
    }

    private IEnumerator LoadSceneAsync()
    {
        yield return null; // odczekaj jedną klatkę, żeby UI się zaktualizowało

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;

            if (progressText != null)
                progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            if (asyncLoad.progress >= 0.9f)
            {
                // Mała pauza dla efektu (opcjonalna)
                yield return new WaitForSeconds(0.3f);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
