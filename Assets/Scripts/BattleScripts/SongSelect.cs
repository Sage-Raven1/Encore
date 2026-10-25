using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Application = UnityEngine.Application;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;

public class SongSelect : MonoBehaviour
{
    [Header("Library")]
    public SongLibrary library;

    [Header("Song List")]
    public Transform content;
    public GameObject entryPrefab;
    public TMP_Dropdown genreDropdown;

    [Header("Detail Panel")]
    public Image jacketImage;
    public TMP_Text titleText;
    public TMP_Text artistText;
    public TMP_Text bpmText;
    public Button[] difficultyButtons = new Button[3];
    public TMP_Text[] difficultyNameTexts = new TMP_Text[3];
    public TMP_Text[] difficultyLevelTexts = new TMP_Text[3];
    public TMP_Text[] difficultyBestTexts = new TMP_Text[3];
    public Button playButton;
    public Button backButton;
    public TMP_Text ratingText;

    [Header("Colours")]
    public Color selectedColor = new Color(0.98f, 0.78f, 0.46f);
    public Color normalColor = new Color(0.36f, 0.36f, 0.35f);

    [Header("Preview")]
    public AudioSource previewSource;

    [Header("Scenes")]
#if UNITY_EDITOR
    public UnityEditor.SceneAsset playScene;
    public UnityEditor.SceneAsset backScene;
#endif
    [SerializeField] string playSceneName;
    [SerializeField] string backSceneName;

#if UNITY_EDITOR
    void OnValidate()
    {
        playSceneName = playScene != null ? playScene.name : "";
        backSceneName = backScene != null ? backScene.name : "";
    }
#endif

    readonly List<SongData> filtered = new List<SongData>();
    readonly List<GameObject> entries = new List<GameObject>();
    readonly Dictionary<string, float> bestAcc = new Dictionary<string, float>();
    int songIndex;
    int diffIndex;

    void Start()
    {
        LoadBestScores();
        SetupGenreDropdown();

        if (genreDropdown != null)
            genreDropdown.onValueChanged.AddListener(_ => { songIndex = 0; RebuildList(); });
        if (playButton != null) playButton.onClick.AddListener(PlaySelected);
        if (backButton != null) backButton.onClick.AddListener(GoBack);

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            int d = i;
            if (difficultyButtons[i] != null)
                difficultyButtons[i].onClick.AddListener(() => SelectDifficulty(d));
        }

        RebuildList();
        UpdateRating();
    }

    void Update()
    {
        if (previewSource != null && previewSource.isPlaying && filtered.Count > 0)
        {
            SongData s = filtered[songIndex];
            if (previewSource.time >= s.previewStart + s.previewLength)
                previewSource.time = s.previewStart;
        }

        Keyboard kb = Keyboard.current;
        if (kb == null || filtered.Count == 0) return;

        if (kb.downArrowKey.wasPressedThisFrame) SelectSong(songIndex + 1);
        if (kb.upArrowKey.wasPressedThisFrame) SelectSong(songIndex - 1);
        if (kb.rightArrowKey.wasPressedThisFrame) SelectDifficulty(NextDifficulty(1));
        if (kb.leftArrowKey.wasPressedThisFrame) SelectDifficulty(NextDifficulty(-1));
        if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame) PlaySelected();
        if (kb.escapeKey.wasPressedThisFrame) GoBack();
    }

    void SetupGenreDropdown()
    {
        if (genreDropdown == null) return;

        List<string> genres = new List<string> { "All" };
        if (library != null)
        {
            genres.AddRange(library.songs
                .Where(s => s != null && !string.IsNullOrWhiteSpace(s.genre))
                .Select(s => s.genre.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g));
        }

        genreDropdown.ClearOptions();
        genreDropdown.AddOptions(genres);
        genreDropdown.value = 0;
        genreDropdown.RefreshShownValue();
    }

    string SelectedGenre()
    {
        if (genreDropdown == null || genreDropdown.value == 0) return null;
        return genreDropdown.options[genreDropdown.value].text;
    }

    void RebuildList()
    {
        foreach (GameObject e in entries)
            if (e != null) Destroy(e);
        entries.Clear();
        filtered.Clear();

        if (library == null || content == null || entryPrefab == null)
        {
            ClearDetail();
            return;
        }

        string genre = SelectedGenre();
        foreach (SongData song in library.songs)
        {
            if (song == null) continue;
            if (genre != null && !string.Equals(song.genre, genre, StringComparison.OrdinalIgnoreCase)) continue;
            filtered.Add(song);
        }

        for (int i = 0; i < filtered.Count; i++)
        {
            int index = i;
            GameObject entry = Instantiate(entryPrefab, content);
            entries.Add(entry);
            SetupEntry(entry, filtered[i]);

            Button b = entry.GetComponent<Button>();
            if (b != null) b.onClick.AddListener(() => SelectSong(index));
        }

        if (filtered.Count == 0)
        {
            ClearDetail();
            return;
        }

        songIndex = Mathf.Clamp(songIndex, 0, filtered.Count - 1);
        SelectSong(songIndex);
    }

    void SetupEntry(GameObject entry, SongData song)
    {
        TMP_Text[] texts = entry.GetComponentsInChildren<TMP_Text>(true);
        if (texts.Length > 0) texts[0].text = song.title;
        if (texts.Length > 1) texts[1].text = song.artist;

        foreach (Image img in entry.GetComponentsInChildren<Image>(true))
        {
            if (img.gameObject == entry) continue;
            img.sprite = song.jacket;
            img.preserveAspect = true;
            break;
        }
    }

    void SelectSong(int index)
    {
        if (filtered.Count == 0) return;
        songIndex = Mathf.Clamp(index, 0, filtered.Count - 1);
        SongData song = filtered[songIndex];

        for (int i = 0; i < entries.Count; i++)
        {
            Image bg = entries[i] != null ? entries[i].GetComponent<Image>() : null;
            if (bg != null) bg.color = i == songIndex ? selectedColor : normalColor;
        }

        if (jacketImage != null)
        {
            jacketImage.sprite = song.jacket;
            jacketImage.preserveAspect = true;
        }
        if (titleText != null) titleText.text = song.title;
        if (artistText != null) artistText.text = song.artist;
        if (bpmText != null) bpmText.text = "BPM " + Mathf.RoundToInt(song.bpm);

        diffIndex = FirstAvailableDifficulty(song, diffIndex);
        RefreshDifficulties(song);
        PlayPreview(song);
    }

    void SelectDifficulty(int i)
    {
        if (filtered.Count == 0) return;
        SongData song = filtered[songIndex];
        if (!HasChart(song, i)) return;
        diffIndex = i;
        RefreshDifficulties(song);
    }

    int NextDifficulty(int dir)
    {
        SongData song = filtered[songIndex];
        int count = song.difficulties.Length;
        for (int step = 1; step <= count; step++)
        {
            int i = ((diffIndex + dir * step) % count + count) % count;
            if (HasChart(song, i)) return i;
        }
        return diffIndex;
    }

    void RefreshDifficulties(SongData song)
    {
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            bool exists = song.difficulties != null && i < song.difficulties.Length;
            bool available = exists && HasChart(song, i);
            DifficultyInfo info = exists ? song.difficulties[i] : null;

            if (difficultyButtons[i] != null)
            {
                difficultyButtons[i].gameObject.SetActive(exists);
                difficultyButtons[i].interactable = available;
                if (difficultyButtons[i].image != null)
                    difficultyButtons[i].image.color = i == diffIndex ? selectedColor : normalColor;
            }

            if (i < difficultyNameTexts.Length && difficultyNameTexts[i] != null)
                difficultyNameTexts[i].text = info != null ? info.name : "";

            if (i < difficultyLevelTexts.Length && difficultyLevelTexts[i] != null)
                difficultyLevelTexts[i].text = info != null ? info.level.ToString("0.0") : "";

            if (i < difficultyBestTexts.Length && difficultyBestTexts[i] != null)
                difficultyBestTexts[i].text = available ? BestText(song, i) : "Not available";
        }

        if (playButton != null)
            playButton.interactable = HasChart(song, diffIndex);
    }

    string BestText(SongData song, int diff)
    {
        if (!bestAcc.TryGetValue(ChartKey(song.title, song.difficulties[diff].name), out float acc))
            return "Not played";
        return acc.ToString("F1") + "% " + GetRank(acc);
    }

    void ClearDetail()
    {
        if (titleText != null) titleText.text = "No songs";
        if (artistText != null) artistText.text = "";
        if (bpmText != null) bpmText.text = "";
        if (jacketImage != null) jacketImage.sprite = null;
    }

    int FirstAvailableDifficulty(SongData song, int preferred)
    {
        if (HasChart(song, preferred)) return preferred;
        for (int i = 0; i < song.difficulties.Length; i++)
            if (HasChart(song, i)) return i;
        return 0;
    }

    bool HasChart(SongData song, int i)
    {
        return song.difficulties != null && i >= 0 && i < song.difficulties.Length
            && song.difficulties[i].chart != null;
    }

    void PlayPreview(SongData song)
    {
        if (previewSource == null || song.audio == null) return;
        if (previewSource.clip == song.audio && previewSource.isPlaying) return;

        previewSource.clip = song.audio;
        previewSource.loop = false;
        previewSource.time = Mathf.Clamp(song.previewStart, 0f, Mathf.Max(0f, song.audio.length - 0.1f));
        previewSource.Play();
    }

    void PlaySelected()
    {
        if (filtered.Count == 0) return;
        SongData song = filtered[songIndex];
        if (!HasChart(song, diffIndex))
        {
            Debug.LogWarning("SongSelect: no chart for that difficulty.");
            return;
        }

        if (string.IsNullOrEmpty(playSceneName))
        {
            Debug.LogWarning("SongSelect: no play scene assigned.");
            return;
        }

        GameSession.Mode = GameMode.Freeplay;
        GameSession.SelectedSong = song;
        GameSession.SelectedDifficulty = diffIndex;
        Time.timeScale = 1f;
        SceneManager.LoadScene(playSceneName);
    }

    void GoBack()
    {
        if (string.IsNullOrEmpty(backSceneName))
        {
            Debug.LogWarning("SongSelect: no back scene assigned.");
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(backSceneName);
    }

    void LoadBestScores()
    {
        bestAcc.Clear();
        string path = Path.Combine(Application.persistentDataPath, "scores.csv");
        if (!File.Exists(path)) return;

        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            if (cols.Length < 10) continue;
            if (!float.TryParse(cols[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float acc))
                continue;

            string key = ChartKey(cols[8].Trim(), cols[9].Trim());
            if (!bestAcc.TryGetValue(key, out float best) || acc > best)
                bestAcc[key] = acc;
        }
    }

    void UpdateRating()
    {
        if (ratingText == null || library == null) return;

        List<float> ratings = new List<float>();
        foreach (SongData song in library.songs)
        {
            if (song == null || song.difficulties == null) continue;
            foreach (DifficultyInfo d in song.difficulties)
            {
                if (d.chart == null) continue;
                if (!bestAcc.TryGetValue(ChartKey(song.title, d.name), out float acc)) continue;
                ratings.Add(d.level * (acc / 100f) * RankBonus(acc));
            }
        }

        ratings.Sort((a, b) => b.CompareTo(a));
        float sum = 0f;
        for (int i = 0; i < Mathf.Min(20, ratings.Count); i++)
            sum += ratings[i];

        ratingText.text = "Rating " + (sum / 20f).ToString("F2");
    }

    static string ChartKey(string title, string difficulty)
    {
        return title.Trim().ToLowerInvariant() + "|" + difficulty.Trim().ToLowerInvariant();
    }

    static string GetRank(float acc)
    {
        if (acc >= 95f) return "S";
        if (acc >= 85f) return "A";
        if (acc >= 75f) return "B";
        if (acc >= 50f) return "C";
        if (acc >= 40f) return "D";
        return "F";
    }

    static float RankBonus(float acc)
    {
        string rank = GetRank(acc);
        if (rank == "S") return 1.0f;
        if (rank == "A") return 0.9f;
        if (rank == "B") return 0.8f;
        if (rank == "C") return 0.6f;
        return 0f;
    }
}