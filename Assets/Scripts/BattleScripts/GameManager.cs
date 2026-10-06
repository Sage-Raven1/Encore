using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //references
    public NoteSpawner spawner;        
    public bool StartPlaying;

    // accuracy weighting
    public float perfectWeight = 1.0f;
    public float greatWeight = 0.66f;
    public float goodWeight = 0.33f;

    float earnedPoints;
    int notesPlayed;
    public float currentAccuracy;

    //combo and multiplier
    public int currentCombo;
    public int longestCombo;
    public int Currentmultiplier;
    public int multiplierTracker;
    public int[] multiplierThreshold;

    [Header("UI Text")]
    public TMP_Text accuracyText;
    public TMP_Text multiText;
    public TMP_Text comboText;

    // note tally
    public int TotalNotes;
    public int goodHits;
    public int greatHits;
    public int perfectHits;
    public int missHits;

    // pass fail threshold
    public float passThreshold = 50f;

    // pause menu
    public GameObject pausePanel;
    public bool isPaused;

    // scenes
    public string resultsSceneName = "Ranking Display";
    public string gameplaySceneName = "Round1";

    bool songFinished;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Currentmultiplier = 1;
        UpdateAccuracyText();
        TotalNotes = spawner.chart.Count;   // count comes from the chart now
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
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
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

        if (Currentmultiplier - 1 < multiplierThreshold.Length)
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(resultsSceneName);
    }

    void SaveScoreToCSV()
    {
        string path = Path.Combine(Application.persistentDataPath, "scores.csv");
        string result = currentAccuracy >= passThreshold ? "PASS" : "FAIL";
        string row = currentAccuracy.ToString("F1") + "," +
                     perfectHits + "," + greatHits + "," + goodHits + "," +
                     missHits + "," + longestCombo + "," + result + "\n";
        try
        {
            if (!File.Exists(path))
                File.WriteAllText(path,
                    "Accuracy,Perfects,Greats,Goods,Misses,LongestCombo,Result\n");
            File.AppendAllText(path, row);
        }
        catch (IOException e)
        {
            Debug.LogWarning("Could not write score: " + e.Message);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (Conductor.instance.song != null) Conductor.instance.song.Pause();
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (Conductor.instance.song != null) Conductor.instance.song.UnPause();
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void RestartSong()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}