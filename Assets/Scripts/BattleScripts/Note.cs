using UnityEngine;

public class Note : MonoBehaviour
{
    public float targetTime;   // when this note should be hit
    public int lane;

    bool obtained = false;

    // timing windows IN SECONDS (not distance!)
    public float perfectWindow = 0.05f;
    public float greatWindow = 0.10f;
    public float goodWindow = 0.15f;

    void Update()
    {
        // position is derived from time-to-hit and scroll speed
        float now = Conductor.instance.SongTime;
        float timeLeft = targetTime - now;
        float y = Conductor.instance.hitLineY + timeLeft * Conductor.instance.scrollSpeed;
        transform.position = new Vector3(transform.position.x, y, 0f);

        KeyCode[] tempKeys = { KeyCode.A, KeyCode.S, KeyCode.L, KeyCode.Semicolon };
        if (!obtained && Input.GetKeyDown(tempKeys[lane]))
        {
            float error = Mathf.Abs(targetTime - now);
            if (error <= goodWindow)      // only judge if within the widest window
            {
                obtained = true;
                if (error <= perfectWindow) GameManager.instance.PerfectHit();
                else if (error <= greatWindow) GameManager.instance.GreatHit();
                else GameManager.instance.GoodHit();
                gameObject.SetActive(false);
            }
        }

        // missed: note fell past the hit line beyond the good window
        if (!obtained && now - targetTime > goodWindow)
        {
            obtained = true;
            GameManager.instance.NoteMissed();
            gameObject.SetActive(false);
        }
    }
}