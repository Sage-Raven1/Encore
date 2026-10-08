using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor instance;

    public AudioSource song;
    public float scrollSpeed = 5f;     // adjustable! units per second
    public float hitLineY = -3.4f;     // where notes are judged
    public float spawnY = 6f;          // where notes appear

    double dspStart;
    double pauseStartDsp;

    public float SongTime { get; private set; }
    public bool Playing { get; private set; }

    void Awake() { instance = this; }

    public void StartSong()
    {
        dspStart = AudioSettings.dspTime;
        if (song != null) song.Play();
        Playing = true;
    }

    // dspTime keeps running while paused, so we shift the start time
    // forward by however long we were paused. Otherwise notes jump ahead
    // and get counted as misses after resuming.
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
            SongTime = (float)(AudioSettings.dspTime - dspStart);
    }

    // how long a note takes to fall from spawn to hit line, at current speed
    public float TravelTime => (spawnY - hitLineY) / scrollSpeed;
}