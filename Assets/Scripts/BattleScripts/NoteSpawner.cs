using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BeatNote
{
    public float beatTime;
    public int lane;
}

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public float[] laneX = new float[4];
    public string chartFileName = "chart";   
    public List<BeatNote> chart = new List<BeatNote>();

    int nextIndex = 0;

    void Awake()
    {
        LoadChart();
    }

    void LoadChart()
    {
        chart.Clear();
        TextAsset file = Resources.Load<TextAsset>(chartFileName);
        if (file == null)
        {
            Debug.LogError("Chart file not found in Resources: " + chartFileName);
            return;
        }

        string[] lines = file.text.Split('\n');
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;   

            string[] parts = trimmed.Split(',');
            if (parts.Length < 2) continue;                

            if (float.TryParse(parts[0], out float time) &&
                int.TryParse(parts[1], out int lane))
            {
                chart.Add(new BeatNote { beatTime = time, lane = lane });
            }
        }

        chart.Sort((a, b) => a.beatTime.CompareTo(b.beatTime));  
        Debug.Log("Loaded " + chart.Count + " notes from chart.");
    }

    void Update()
    {
        if (!Conductor.instance.Playing) return;

        float now = Conductor.instance.SongTime;
        float travel = Conductor.instance.TravelTime;

        while (nextIndex < chart.Count &&
               chart[nextIndex].beatTime - now <= travel)
        {
            Spawn(chart[nextIndex]);
            nextIndex++;
        }
    }

    void Spawn(BeatNote data)
    {
        GameObject go = Instantiate(notePrefab);
        Note note = go.GetComponent<Note>();
        if (note == null)
        {
            Debug.LogError("Note prefab is missing the Note script!");
            return;
        }
        note.targetTime = data.beatTime;
        note.lane = data.lane;
        go.transform.position = new Vector3(
            laneX[data.lane], Conductor.instance.spawnY, 0f);
    }
}