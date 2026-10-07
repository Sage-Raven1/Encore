using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    public Conductor conductor;
    public GameObject notePrefab;
    public TextAsset chartCSV;  // DRAG YOUR CSV FILE HERE
    public List<NoteData> chartNotes = new List<NoteData>();

    // Spawn settings
    public float[] laneXPositions = new float[] { -1.53f, -0.47f, 0.52f, 1.51f };
    public float spawnY = 6f;
    public float hitLineY = -3.4f;

    private Dictionary<int, Queue<Note>> notesInLanes = new Dictionary<int, Queue<Note>>();

    void Start()
    {
        // Find Conductor if not assigned
        if (conductor == null)
            conductor = Conductor.instance;

        if (conductor == null)
        {
            UnityEngine.Debug.LogError("NoteSpawner: Conductor not found!");
            return;
        }

        // Initialize note queues for each lane
        for (int i = 0; i < 4; i++)
        {
            notesInLanes[i] = new Queue<Note>();
        }

        // Load chart from CSV if assigned
        if (chartCSV != null)
        {
            LoadChartFromCSV(chartCSV);
            UnityEngine.Debug.Log($"NoteSpawner: Loaded {chartNotes.Count} notes from CSV");
        }
        else
        {
            UnityEngine.Debug.LogWarning("NoteSpawner: No chart CSV assigned! Drag a CSV file into the chartCSV field.");
        }
    }

    void Update()
    {
        // Don't spawn notes while paused
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        if (conductor == null || chartNotes == null || chartNotes.Count == 0)
            return;

        float currentTime = conductor.SongTime;

        // Spawn notes that are within the spawn window (5 seconds ahead)
        foreach (NoteData note in chartNotes)
        {
            if (!note.hasBeenSpawned && note.time <= currentTime + 5f)
            {
                SpawnNote(note);
                note.hasBeenSpawned = true;
            }
        }
    }

    void SpawnNote(NoteData noteData)
    {
        if (notePrefab == null)
        {
            UnityEngine.Debug.LogError("Note prefab not assigned!");
            return;
        }

        // Get the X position for this lane
        if (noteData.lane < 0 || noteData.lane >= laneXPositions.Length)
        {
            UnityEngine.Debug.LogError($"Invalid lane: {noteData.lane}");
            return;
        }

        float xPos = laneXPositions[noteData.lane];

        // Instantiate the note at spawn position
        GameObject noteObj = Instantiate(notePrefab, new Vector3(xPos, spawnY, 0), Quaternion.identity);
        Note noteScript = noteObj.GetComponent<Note>();

        if (noteScript == null)
        {
            UnityEngine.Debug.LogError("Note prefab missing Note script!");
            Destroy(noteObj);
            return;
        }

        // Set up the note with timing data
        noteScript.lane = noteData.lane;
        noteScript.targetTime = noteData.time;
        noteScript.hasBeenHit = false;

        // Add to lane queue for tracking
        notesInLanes[noteData.lane].Enqueue(noteScript);

        UnityEngine.Debug.Log($"Spawned note at time {noteData.time} in lane {noteData.lane}");
    }

    public void LoadChartFromCSV(TextAsset csvFile)
    {
        if (csvFile == null)
        {
            UnityEngine.Debug.LogError("CSV file is null!");
            return;
        }

        chartNotes.Clear();
        string[] lines = csvFile.text.Split('\n');

        // Skip header line if it exists
        int startLine = (lines.Length > 0 && lines[0].Contains("time")) ? 1 : 0;

        for (int i = startLine; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] parts = line.Split(',');
            if (parts.Length >= 2)
            {
                if (float.TryParse(parts[0], out float time) && int.TryParse(parts[1], out int lane))
                {
                    if (lane >= 0 && lane < 4)
                    {
                        chartNotes.Add(new NoteData { time = time, lane = lane, hasBeenSpawned = false });
                    }
                }
            }
        }

        // Sort by time
        chartNotes.Sort((a, b) => a.time.CompareTo(b.time));

        UnityEngine.Debug.Log($"Loaded {chartNotes.Count} notes from chart: {csvFile.name}");
    }
}

public class NoteData
{
    public float time;
    public int lane;
    public bool hasBeenSpawned;
}