using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;

    [Header("Button Scenes")]
#if UNITY_EDITOR
    public UnityEditor.SceneAsset storyScene;
    public UnityEditor.SceneAsset freeplayScene;
#endif
    [SerializeField] string storySceneName;
    [SerializeField] string freeplaySceneName;

#if UNITY_EDITOR
    void OnValidate()
    {
        storySceneName = storyScene != null ? storyScene.name : "";
        freeplaySceneName = freeplayScene != null ? freeplayScene.name : "";
    }
#endif

    void Start()
    {
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
    }

    public void StartStory()
    {
        GameSession.Mode = GameMode.Story;
        GameSession.SelectedSong = null;
        LoadScene(storySceneName, "Story");
    }

    public void StartFreeplay()
    {
        GameSession.Mode = GameMode.Freeplay;
        LoadScene(freeplaySceneName, "Freeplay");
    }

    public void OpenSettings()
    {
        if (SettingsPanel != null) SettingsPanel.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    public void LoadScene(string sceneName, string label)
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