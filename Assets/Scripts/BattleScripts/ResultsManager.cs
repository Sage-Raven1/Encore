using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.IO;
using TMPro;
using Debug = UnityEngine.Debug;
using Application = UnityEngine.Application;

public class ResultsManager : MonoBehaviour
{
    public TMP_Text perfectDisplay;
    public TMP_Text greatDisplay;
    public TMP_Text goodDisplay;
    public TMP_Text missDisplay;
    public TMP_Text accuracyDisplay;
    public TMP_Text longestComboDisplay;
    public TMP_Text rankingDisplay;
    public TMP_Text passFailDisplay;

    [Header("Personal Record")]
    public TMP_Text personalBestDisplay;
    public TMP_Text newRecordDisplay;
    public TMP_Text chartInfoDisplay;

    [Header("Continue Scene")]
#if UNITY_EDITOR
    public UnityEditor.SceneAsset returnScene;
#endif
    [SerializeField] string returnSceneName;

    [Header("Retry Scene")]
#if UNITY_EDITOR
    public UnityEditor.SceneAsset retryScene;
#endif
    [SerializeField] string retrySceneName;

    string playedRound;
    string playedSong;
    string playedDifficulty;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (returnScene != null)
            returnSceneName = returnScene.name;
        retrySceneName = retryScene != null ? retryScene.name : "";
    }
#endif

    void Start()
    {
        Time.timeScale = 1f;
        LoadLatestScore();
    }

    void LoadLatestScore()
    {
        string path = Path.Combine(Application.persistentDataPath, "scores.csv");

        if (!File.Exists(path)) return;

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2) return;

        string[] cols = lines[lines.Length - 1].Split(',');
        if (cols.Length < 7) return;

        accuracyDisplay.text = "Accuracy: " + cols[0] + "%";
        perfectDisplay.text = "Perfects: " + cols[1];
        greatDisplay.text = "Greats: " + cols[2];
        goodDisplay.text = "Goods: " + cols[3];
        missDisplay.text = "Misses: " + cols[4];
        longestComboDisplay.text = "Longest combo: " + cols[5];
        passFailDisplay.text = cols[6];
        // indicates which column is which in the CSV file:

        playedRound = cols.Length >= 8 ? cols[7].Trim() : null;
        playedSong = cols.Length >= 10 ? cols[8].Trim() : null;
        playedDifficulty = cols.Length >= 10 ? cols[9].Trim() : null;

        if (chartInfoDisplay != null)
            chartInfoDisplay.text = playedSong != null ? playedSong + " - " + playedDifficulty : "";

        // rank from the accuracy column
        if (TryParseAccuracy(cols[0], out float acc))
        {
            rankingDisplay.text = GetRank(acc);
            ShowPersonalBest(lines, acc);
        }
    }

    void ShowPersonalBest(string[] lines, float currentAcc)
    {
        float previousBest = -1f;

        for (int i = 1; i < lines.Length - 1; i++)
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 10) continue;
            if (cols[8].Trim() != playedSong) continue;
            if (cols[9].Trim() != playedDifficulty) continue;
            if (TryParseAccuracy(cols[0], out float acc) && acc > previousBest)
                previousBest = acc;
        }

        if (newRecordDisplay != null) newRecordDisplay.gameObject.SetActive(false);
        if (personalBestDisplay == null) return;

        if (playedSong == null || previousBest < 0f)
        {
            personalBestDisplay.text = "Personal Best: --";
            if (playedSong != null && newRecordDisplay != null)
            {
                newRecordDisplay.text = "FIRST CLEAR!";
                newRecordDisplay.gameObject.SetActive(true);
            }
            return;
        }

        float diff = currentAcc - previousBest;
        string sign = diff >= 0f ? "+" : "";
        personalBestDisplay.text = "Personal Best: " + previousBest.ToString("F1") + "% (" +
                                   sign + diff.ToString("F1") + "%)";

        if (diff > 0f && newRecordDisplay != null)
        {
            newRecordDisplay.text = "NEW RECORD!";
            newRecordDisplay.gameObject.SetActive(true);
        }
    }

    bool TryParseAccuracy(string text, out float acc)
    {
        return float.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out acc);
    }

    string GetRank(float acc)
    {
        if (acc >= 95f) return "S";
        if (acc >= 85f) return "A";
        if (acc >= 75f) return "B";
        if (acc >= 50f) return "C";
        if (acc >= 40f) return "D";
        return "F";
    }

    public void RetrySong()
    {
        string scene = RoundTransition.RetryScene;
        if (string.IsNullOrEmpty(scene)) scene = retrySceneName;
        if (string.IsNullOrEmpty(scene)) scene = playedRound;
        if (string.IsNullOrEmpty(scene))
        {
            Debug.LogWarning("ResultsManager: No retry scene assigned.");
            return;
        }
        SceneManager.LoadScene(scene);
    }

    public void ReturnToMenu()
    {
        string scene = RoundTransition.ContinueScene;
        if (string.IsNullOrEmpty(scene)) scene = returnSceneName;
        if (string.IsNullOrEmpty(scene))
        {
            Debug.LogWarning("ResultsManager: No continue scene assigned.");
            return;
        }
        SceneManager.LoadScene(scene);
    }
}