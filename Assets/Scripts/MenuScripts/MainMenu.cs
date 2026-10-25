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
#if UNITY_EDITOR
    public UnityEditor.SceneAsset freeplayScene;
#endif
    [SerializeField] string freeplaySceneName;

    [Header("Settings")]
    public SettingsMenu settingsMenu;
    public GameObject settingsPanel;

#if UNITY_EDITOR
    void OnValidate()
    {
        storySceneName = storyScene != null ? storyScene.name : "";
        freeplaySceneName = freeplayScene != null ? freeplayScene.name : "";
    }
#endif

    void Start()
    {
        Time.timeScale = 1f;
        if (settingsMenu == null && settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (settingsMenu != null)
            settingsMenu.Open();
        else if (settingsPanel != null)
            settingsPanel.SetActive(true);
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

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

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