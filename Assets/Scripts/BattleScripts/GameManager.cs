using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Application = UnityEngine.Application;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // setting up base values
    public NoteSpawner spawner;
    public bool StartPlaying;

    public float perfectWeight = 1.0f;
    public float greatWeight = 0.66f;
    public float goodWeight = 0.33f;

    float earnedPoints;
    int notesPlayed;
    public float currentAccuracy = 100f;

    public int currentCombo;
    public int longestCombo;
    public int Currentmultiplier;
    public int multiplierTracker;
    public int[] multiplierThreshold;

    // UI Text
    public TMP_Text accuracyText;
    public TMP_Text multiText;
    public TMP_Text comboText;

    public int TotalNotes;
    public int goodHits;
    public int greatHits;
    public int perfectHits;
    public int missHits;

    public float passThreshold = 50f;

    public GameObject pausePanel;
    public bool isPaused;

    [// resume countdown
    public float resumeDelay = 3f;
    public TMP_Text countdownText;
    bool isResuming;

    //chart info
    public string songName;
    public string difficulty;

    // results info
#if UNITY_EDITOR
    public UnityEditor.SceneAsset resultsScene;
#endif
    [SerializeField] string resultsSceneName = "Ranking Display";

    // after results (dependant on which round/ scene)
#if UNITY_EDITOR
    public UnityEditor.SceneAsset afterResultsContinue;
    public UnityEditor.SceneAsset afterResultsRetry;
#endif
    [SerializeField] string afterResultsContinueName;
    [SerializeField] string afterResultsRetryName;

    // restart scene
#if UNITY_EDITOR
    public UnityEditor.SceneAsset restartScene;
#endif
    [SerializeField] string restartSceneName;

    // Quit To Menu
#if UNITY_EDITOR
    public UnityEditor.SceneAsset menuScene;
#endif
    [SerializeField] string menuSceneName;

    bool songFinished;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (menuScene != null)
            menuSceneName = menuScene.name;
        if (resultsScene != null)
            resultsSceneName = resultsScene.name;
        restartSceneName = restartScene != null ? restartScene.name : "";
        afterResultsContinueName = afterResultsContinue != null ? afterResultsContinue.name : "";
        afterResultsRetryName = afterResultsRetry != null ? afterResultsRetry.name : "";
    }
#endif

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Currentmultiplier = 1;
        RefreshUI();
        TotalNotes = spawner != null ? spawner.chartNotes.Count : 0;
    }

    void Update()
    {
        if (!StartPlaying)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                StartPlaying = true;
                Conductor.instance.StartSong();
            }
        }
        else
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && !isResuming)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    void RegisterHit(float weight)
    {
        earnedPoints += weight;
        notesPlayed++;

        currentCombo++;
        if (currentCombo > longestCombo)
            longestCombo = currentCombo;

        if (multiplierThreshold != null && Currentmultiplier - 1 < multiplierThreshold.Length)
        {
            multiplierTracker++;
            if (multiplierThreshold[Currentmultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                Currentmultiplier++;
            }
        }

        RefreshUI();
        CheckSongComplete();
    }

    public void PerfectHit() { perfectHits++; RegisterHit(perfectWeight); }
    public void GreatHit() { greatHits++; RegisterHit(greatWeight); }
    public void GoodHit() { goodHits++; RegisterHit(goodWeight); }

    public void NoteMissed()
    {
        missHits++;
        notesPlayed++;
        currentCombo = 0;
        Currentmultiplier = 1;
        multiplierTracker = 0;

        RefreshUI();
        CheckSongComplete();
    }

    void RecalculateAccuracy()
    {
        if (notesPlayed > 0)
            currentAccuracy = (earnedPoints / notesPlayed) * 100f;
        else
            currentAccuracy = 100f;
    }

    void RefreshUI()
    {
        RecalculateAccuracy();
        UpdateAccuracyText();
        if (comboText != null) comboText.text = "Combo: " + currentCombo;
        if (multiText != null) multiText.text = "Multiplier: x" + Currentmultiplier;
    }

    void UpdateAccuracyText()
    {
        if (accuracyText != null)
            accuracyText.text = "Accuracy: " + currentAccuracy.ToString("F1") + "%";
    }

    void CheckSongComplete()
    {
        if (songFinished) return;
        if (notesPlayed >= TotalNotes && TotalNotes > 0)
        {
            songFinished = true;
            SongComplete();
        }
    }

    void SongComplete()
    {
        SaveScoreToCSV();
        RoundTransition.ContinueScene = afterResultsContinueName;
        RoundTransition.RetryScene = afterResultsRetryName;
        Time.timeScale = 1f;
        SceneManager.LoadScene(resultsSceneName);
    }

    void SaveScoreToCSV()
    {
        string path = Path.Combine(Application.persistentDataPath, "scores.csv");
        string result = currentAccuracy >= passThreshold ? "PASS" : "FAIL";
        string row = currentAccuracy.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "," +
                     perfectHits + "," + greatHits + "," + goodHits + "," +
                     missHits + "," + longestCombo + "," + result + "," +
                     SceneManager.GetActiveScene().name + "," +
                     CleanField(GetSongName()) + "," + CleanField(difficulty) + "\n";
        try
        {
            if (!File.Exists(path))
                File.WriteAllText(path,
                    "Accuracy,Perfects,Greats,Goods,Misses,LongestCombo,Result,Round,Song,Difficulty\n");
            File.AppendAllText(path, row);
        }
        catch (IOException e)
        {
            Debug.LogWarning("Could not write score: " + e.Message);
        }
    }

    string GetSongName()
    {
        if (!string.IsNullOrEmpty(songName)) return songName;
        if (spawner != null && spawner.chartCSV != null) return spawner.chartCSV.name;
        return SceneManager.GetActiveScene().name;
    }

    string CleanField(string value)
    {
        if (string.IsNullOrEmpty(value)) return "Normal";
        return value.Replace(",", " ").Replace("\n", " ").Replace("\r", " ").Trim();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Conductor.instance.Pause();
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!isPaused || isResuming) return;
        StartCoroutine(ResumeCountdown());
    }

    IEnumerator ResumeCountdown()
    {
        isResuming = true;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        float remaining = resumeDelay;
        while (remaining > 0f)
        {
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(remaining).ToString();
            yield return null;
            remaining -= Time.unscaledDeltaTime;
        }

        if (countdownText != null) countdownText.gameObject.SetActive(false);

        isPaused = false;
        isResuming = false;
        Time.timeScale = 1f;
        Conductor.instance.Resume();
    }

    public void RestartSong()
    {
        Time.timeScale = 1f;
        if (string.IsNullOrEmpty(restartSceneName))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(restartSceneName);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        if (string.IsNullOrEmpty(menuSceneName))
        {
            Debug.LogWarning("GameManager: No menu scene assigned.");
            return;
        }
        SceneManager.LoadScene(menuSceneName);
    }
}