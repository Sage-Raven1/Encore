using UnityEngine;

[System.Serializable]
public class DifficultyInfo
{
    public string name = "Normal";
    public float level = 5f;
    public TextAsset chart;
}

[CreateAssetMenu(fileName = "NewSong", menuName = "Encore/Song Data")]
public class SongData : ScriptableObject
{
    public string title;
    public string artist;
    public string genre = "Vocaloid";
    public float bpm = 120f;
    public Sprite jacket;
    public AudioClip audio;
    public float previewStart = 30f;
    public float previewLength = 15f;

    public DifficultyInfo[] difficulties =
    {
        new DifficultyInfo { name = "Easy", level = 3f },
        new DifficultyInfo { name = "Normal", level = 6f },
        new DifficultyInfo { name = "Hard", level = 9f }
    };
}