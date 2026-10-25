using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using Debug = UnityEngine.Debug;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string storySceneName;
    [SerializeField] private string freeplaySceneName;
    public GameObject SettingsPanel;


    void Start()
    {
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
    }

    // start story function -> need to attatch to the story button
    public void StartStory()
    {
        LoadScene(storySceneName, "Story");
    }

    // same thing as with story
    public void StartFreeplay()
    {
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
