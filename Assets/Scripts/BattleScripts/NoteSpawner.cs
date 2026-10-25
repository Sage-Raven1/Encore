using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

// Force Unity's Debug even if an IDE auto-adds System.Diagnostics
using Debug = UnityEngine.Debug;

public class NoteSpawner : MonoBehaviour
{
    public Conductor conductor;
    public GameObject notePrefab;
    public TextAsset chartCSV;
    public List<NoteData> chartNotes = new List<NoteData>();

    // Spawn settings
    public float[] laneXPositions = new float[] { -1.52f, -0.52f, 0.52f, 1.52f };
    public float spawnY = 6f;
    public float hitLineY = -3.4f;

    int nextNoteIndex = 0;   // chart is sorted, so we only need to look at the next note

    void Awake()
    {
        // Load in Awake so GameManager.Start can read chartNotes.Count
        if (chartCSV != null)
            LoadChartFromCSV(chartCSV);
        else
            Debug.LogWarning("NoteSpawner: No chart CSV assigned! Drag a CSV file into the chartCSV field.");
    }

    void Start()
    {
        if (conductor == null)
            conductor = Conductor.instance;
    }

    void Update()
    {
        // Don't spawn notes while paused
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        if (conductor == null || chartNotes == null || chartNotes.Count == 0)
            return;

        // Spawn each note just as it would enter the screen at the top
        float spawnAhead = conductor.TravelTime;
        float currentTime = conductor.SongTime;

        while (nextNoteIndex < chartNotes.Count &&
               chartNotes[nextNoteIndex].time <= currentTime + spawnAhead)
        {
            SpawnNote(chartNotes[nextNoteIndex]);
            chartNotes[nextNoteIndex].hasBeenSpawned = true;
            nextNoteIndex++;
        }
    }

    void SpawnNote(NoteData noteData)
    {
        if (notePrefab == null)
        {
            Debug.LogError("Note prefab not assigned!");
            return;
        }

        if (noteData.lane < 0 || noteData.lane >= laneXPositions.Length)
        {
            Debug.LogWarning($"Invalid lane: {noteData.lane}");
            return;
        }

        float xPos = laneXPositions[noteData.lane];

        GameObject noteObj = Instantiate(notePrefab, new Vector3(xPos, spawnY, 0), Quaternion.identity);
        Note noteScript = noteObj.GetComponent<Note>();

        if (noteScript == null)
        {
            Debug.LogError("Note prefab missing Note script!");
            Destroy(noteObj);
            return;
        }

        noteScript.lane = noteData.lane;
        noteScript.targetTime = noteData.time;
    }

    public void LoadChartFromCSV(TextAsset csvFile)
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV file is null!");
            return;
        }

        chartNotes.Clear();
        nextNoteIndex = 0;
        string[] lines = csvFile.text.Split('\n');

        // Skip header line if it exists
        int startLine = (lines.Length > 0 && lines[0].Contains("time")) ? 1 : 0;

        for (int i = startLine; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] parts = line.Split(',');
            if (parts.Length >= 2 &&
                float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float time) &&
                int.TryParse(parts[1].Trim(), out int lane) &&
                lane >= 0 && lane < 4)
            {
                chartNotes.Add(new NoteData { time = time, lane = lane, hasBeenSpawned = false });
            }
        }

        chartNotes.Sort((a, b) => a.time.CompareTo(b.time));

        Debug.Log($"Loaded {chartNotes.Count} notes from chart: {csvFile.name}");
    }
}

[System.Serializable]
public class NoteData
{
    public float time;
    public int lane;
    public bool hasBeenSpawned;
}