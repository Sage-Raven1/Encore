using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //references
    public NoteSpawner spawner;        // drag NoteSpawner in
    public Conductor conductor;
    public JudgementDisplay judgementDisplay;
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

        if (conductor == null)
            conductor = Conductor.instance;

        if (judgementDisplay == null)
            judgementDisplay = FindObjectOfType<JudgementDisplay>();

        // Count notes from spawner's chart
        if (spawner != null && spawner.chartNotes != null)
            TotalNotes = spawner.chartNotes.Count;

        // ENSURE PAUSE PANEL IS INVISIBLE AT START
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (!StartPlaying)
        {
            // press any key to begin (new Input System)
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                StartPlaying = true;
                conductor.StartSong();
            }
        }
        else
        {
            // Pause with ESC
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }

            // Check for lane hits
            if (!isPaused)
            {
                if (Keyboard.current.dKey.wasPressedThisFrame)
                    OnLaneHit(0);

                if (Keyboard.current.fKey.wasPressedThisFrame)
                    OnLaneHit(1);

                if (Keyboard.current.jKey.wasPressedThisFrame)
                    OnLaneHit(2);

                if (Keyboard.current.kKey.wasPressedThisFrame)
                    OnLaneHit(3);
            }
        }
    }

    public void OnLaneHit(int lane)
    {
        if (!StartPlaying || isPaused) return;

        float currentTime = conductor.SongTime;
        Note[] allNotes = FindObjectsOfType<Note>();

        UnityEngine.Debug.Log($"Lane {lane} hit at time {currentTime}. Found {allNotes.Length} notes total.");

        foreach (Note note in allNotes)
        {
            if (note.lane == lane && !note.hasBeenHit)
            {
                float timeDiff = Mathf.Abs(note.targetTime - currentTime);
                UnityEngine.Debug.Log($"Note in lane {lane}: targetTime={note.targetTime}, currentTime={currentTime}, timeDiff={timeDiff}");

                // Determine judgement based on timing
                if (timeDiff < 0.05f)
                {
                    UnityEngine.Debug.Log("PERFECT HIT!");
                    PerfectHit();
                    note.OnHit();
                    if (judgementDisplay != null)
                        judgementDisplay.ShowJudgement("Perfect");
                    return;
                }
                else if (timeDiff < 0.1f)
                {
                    UnityEngine.Debug.Log("GREAT HIT!");
                    GreatHit();
                    note.OnHit();
                    if (judgementDisplay != null)
                        judgementDisplay.ShowJudgement("Great");
                    return;
                }
                else if (timeDiff < 0.15f)
                {
                    UnityEngine.Debug.Log("GOOD HIT!");
                    GoodHit();
                    note.OnHit();
                    if (judgementDisplay != null)
                        judgementDisplay.ShowJudgement("Good");
                    return;
                }
            }
        }
    }

    void RegisterHit(float weight)
    {
        earnedPoints += weight;
        notesPlayed++;

        UnityEngine.Debug.Log($"Hit registered! earnedPoints={earnedPoints}, notesPlayed={notesPlayed}");

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

    public void PerfectHit()
    {
        perfectHits++;
        RegisterHit(perfectWeight);
    }

    public void GreatHit()
    {
        greatHits++;
        RegisterHit(greatWeight);
    }

    public void GoodHit()
    {
        goodHits++;
        RegisterHit(goodWeight);
    }

    public void NoteMissed()
    {
        missHits++;
        notesPlayed++;
        currentCombo = 0;
        Currentmultiplier = 1;
        multiplierTracker = 0;

        if (judgementDisplay != null)
            judgementDisplay.ShowJudgement("Miss");

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
        string path = Path.Combine(UnityEngine.Application.persistentDataPath, "scores.csv");
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
            UnityEngine.Debug.LogWarning("Could not write score: " + e.Message);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (Conductor.instance != null) Conductor.instance.PauseSong();
        if (pausePanel != null) pausePanel.SetActive(true);  // Show pause panel
        UnityEngine.Debug.Log("Game Paused - Pause Panel Shown");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (Conductor.instance != null) Conductor.instance.ResumeSong();
        if (pausePanel != null) pausePanel.SetActive(false);  // Hide pause panel
        UnityEngine.Debug.Log("Game Resumed - Pause Panel Hidden");
    }

    public void RestartSong()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.Application.Quit();
    }
}