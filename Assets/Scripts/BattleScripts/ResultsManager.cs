using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

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

    public string gameplaySceneName = "Round1";

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

        // rank from the accuracy column
        if (float.TryParse(cols[0], out float acc))
            rankingDisplay.text = GetRank(acc);
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
        SceneManager.LoadScene(gameplaySceneName);
    }
}