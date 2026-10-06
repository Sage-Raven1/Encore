using UnityEngine;

public class PauseButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        // pause if playing
        if (GameManager.instance.isPaused)
            GameManager.instance.ResumeGame();
        else
            GameManager.instance.PauseGame();
    }
}
