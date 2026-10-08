using UnityEngine;
using UnityEngine.InputSystem;

public class Note : MonoBehaviour
{
    public float targetTime;   // when this note should be hit
    public int lane;

    bool obtained = false;

    // timing windows IN SECONDS (not distance!)
    public float perfectWindow = 0.05f;
    public float greatWindow = 0.10f;
    public float goodWindow = 0.15f;

    // lane keys (new Input System, to match GameManager)
    static readonly Key[] laneKeys = { Key.A, Key.S, Key.L, Key.Semicolon };

    void Update()
    {
        if (obtained) return;
        if (GameManager.instance != null && GameManager.instance.isPaused) return;

        // position is derived from time-to-hit and scroll speed
        float now = Conductor.instance.SongTime;
        float timeLeft = targetTime - now;
        float y = Conductor.instance.hitLineY + timeLeft * Conductor.instance.scrollSpeed;
        transform.position = new Vector3(transform.position.x, y, 0f);

        if (!Conductor.instance.Playing) return;

        var kb = Keyboard.current;
        if (kb != null && lane >= 0 && lane < laneKeys.Length && kb[laneKeys[lane]].wasPressedThisFrame)
        {
            float error = Mathf.Abs(targetTime - now);
            if (error <= goodWindow)      // only judge if within the widest window
            {
                obtained = true;
                if (error <= perfectWindow) GameManager.instance.PerfectHit();
                else if (error <= greatWindow) GameManager.instance.GreatHit();
                else GameManager.instance.GoodHit();
                Destroy(gameObject);
                return;
            }
        }

        // missed: note fell past the hit line beyond the good window
        if (now - targetTime > goodWindow)
        {
            obtained = true;
            GameManager.instance.NoteMissed();
            Destroy(gameObject);
        }
    }
}