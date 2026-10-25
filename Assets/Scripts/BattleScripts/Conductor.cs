using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor instance;

    public AudioSource song;
    public float scrollSpeed = 5f;
    public float hitLineY = -3.4f;
    public float spawnY = 6f;

    double dspStart;
    double pauseStartDsp;

    public float SongTime { get; private set; }
    public bool Playing { get; private set; }

    void Awake()
    {
        instance = this;
        scrollSpeed = GameSettings.ScrollSpeed;
        if (song != null) song.volume = GameSettings.MusicVolume;
        SongTime = -GameSettings.AudioOffset / 1000f;
    }

    public void StartSong()
    {
        dspStart = AudioSettings.dspTime;
        if (song != null) song.Play();
        Playing = true;
    }

    public void Pause()
    {
        if (!Playing) return;
        pauseStartDsp = AudioSettings.dspTime;
        if (song != null) song.Pause();
        Playing = false;
    }

    public void Resume()
    {
        if (Playing) return;
        dspStart += AudioSettings.dspTime - pauseStartDsp;
        if (song != null) song.UnPause();
        Playing = true;
    }

    void Update()
    {
        if (Playing)
            SongTime = (float)(AudioSettings.dspTime - dspStart) - GameSettings.AudioOffset / 1000f;
    }

    public float TravelTime => (spawnY - hitLineY) / scrollSpeed;
}