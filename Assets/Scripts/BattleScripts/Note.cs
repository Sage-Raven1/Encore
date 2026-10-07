using UnityEngine;

public class Note : MonoBehaviour
{
    public int lane;                    // Which lane (0-3)
    public float targetTime;            // When this note should be hit (in seconds)
    public bool hasBeenHit = false;     // Has this note been hit or missed

    public float scrollSpeed = 20f;     // Speed notes travel down
    public float hitLineY = -3.4f;      // Visual hit line position
    public float spawnY = 6f;           // Spawn position

    void Start()
    {
        // Get scroll speed and hit line position from Conductor if available
        if (Conductor.instance != null)
        {
            scrollSpeed = Conductor.instance.scrollSpeed * 2f;
            hitLineY = Conductor.instance.hitLineY;
        }
    }

    void Update()
    {
        if (hasBeenHit)
            return;

        // Move note down the screen
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        // Check if note has passed the acceptable miss window
        // Only mark as missed if we're well past the hit window
        float currentTime = Conductor.instance != null ? Conductor.instance.SongTime : 0f;
        float timeDiff = currentTime - targetTime;

        // If note is more than 0.2 seconds past its target time, mark as missed
        if (timeDiff > 0.2f && !hasBeenHit)
        {
            OnMiss();
        }
    }

    public void OnHit()
    {
        hasBeenHit = true;
        UnityEngine.Debug.Log($"Note hit! Lane {lane} at time {targetTime}");
        // Immediately destroy the note
        Destroy(gameObject);
    }

    void OnMiss()
    {
        hasBeenHit = true;
        UnityEngine.Debug.Log($"Note missed! Lane {lane} at time {targetTime}");
        if (GameManager.instance != null)
        {
            GameManager.instance.NoteMissed();
        }

        // Destroy the note
        Destroy(gameObject);
    }
}