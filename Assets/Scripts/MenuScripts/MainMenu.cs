using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Application = UnityEngine.Application;

public class MainMenu : MonoBehaviour
{
    [Header("Story Mode")]
#if UNITY_EDITOR
    public UnityEditor.SceneAsset storyScene;
#endif
    [SerializeField] string storySceneName;

    [Header("Freeplay")]
    // freeplay scene (shows up in inspector)
#if UNITY_EDITOR
    public UnityEditor.SceneAsset freeplayScene;
#endif
    [SerializeField] string freeplaySceneName;

#if UNITY_EDITOR
    void OnValidate()
    {
        storySceneName = storyScene != null ? storyScene.name : "";
        freeplaySceneName = freeplayScene != null ? freeplayScene.name : "";
    }
#endif

    // button scene switching/ functionality
    public void StartStory()
    {
        LoadScene(storySceneName, "Story");
    }

    public void StartFreeplay()
    {
        LoadScene(freeplaySceneName, "Freeplay");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // load scene function
    void LoadScene(string sceneName, string label)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("MainMenu: No " + label + " scene assigned.");
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}