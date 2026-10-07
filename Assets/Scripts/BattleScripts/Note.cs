using UnityEngine;

public class Note : MonoBehaviour
{
    public int lane;                    // Which lane (0-3)
    public float targetTime;            // When this note should be hit (in seconds)
    public bool hasBeenHit = false;     // Has this note been hit or missed

    public float scrollSpeed = 10f;     // Speed that notes travel down
    public float hitLineY = -4f;        // Y position of the hit line

    void Start()
    {
        // Get scroll speed and hit line position from Conductor if available
        if (Conductor.instance != null)
        {
            scrollSpeed = Conductor.instance.scrollSpeed;
            hitLineY = Conductor.instance.hitLineY;
        }
    }

    void Update()
    {
        if (hasBeenHit)
            return;

        // Move note down the screen
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        // Check if note has passed the hit line (missed)
        if (transform.position.y < hitLineY && !hasBeenHit)
        {
            OnMiss();
        }
    }

    public void OnHit()
    {
        hasBeenHit = true;
        // Destroy the note after a short delay for visual feedback
        Destroy(gameObject, 0.1f);
    }

    void OnMiss()
    {
        hasBeenHit = true;
        if (GameManager.instance != null)
        {
            GameManager.instance.NoteMissed();
        }

       
        Destroy(gameObject);
    }
}