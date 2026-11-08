using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;
    public bool continuePlay;
    private StatsManager statsManager;

    public Button storymodeButton;
    public Button freeplayButton;
    public Button settingsButton;
    public Button quitButton;

    bool updatingUI;

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
        Debug.Log("MainMenu Init");
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
        statsManager = GetComponent<StatsManager>();
    }

    private void Awake()
    {
        Debug.Log(storymodeButton.name);
        Debug.Log("Main menu awake func");
        storymodeButton.onClick.AddListener(StartStory);
        freeplayButton.onClick.AddListener(StartFreeplay);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void StartStory()
    {
        Debug.Log("Story Method Entered");
        continuePlay = SaveSystem.CheckSaveFile();

        if (continuePlay)
        {
            continuePlay = false;
            //statsManager.LoadPlayer();
            string sceneToLoad = statsManager.currentScene;
            Debug.Log(statsManager.currentScene);
            Debug.Log(sceneToLoad);
            GameSession.Mode = GameMode.Story;
            GameSession.SelectedSong = null;
            LoadScene(sceneToLoad, "Continue Story");
        }
        else
        {
            statsManager.currentScene = "Backstage";
            GameSession.Mode = GameMode.Story;
            GameSession.SelectedSong = null;
            LoadScene(storySceneName, "Story");
        }
    }

    public void StartFreeplay()
    {
        Debug.Log("Freeplay Pressed");
        GameSession.Mode = GameMode.Freeplay;
        LoadScene(freeplaySceneName, "Freeplay");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings Pressed");
        SettingsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Pressed");
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