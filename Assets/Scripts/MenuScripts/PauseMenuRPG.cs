using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuRPG : MonoBehaviour
{
    public Canvas pauseCanvasRPG;

    private void Start()
    {
        pauseCanvasRPG.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseCanvasRPG.enabled)
        {
            pauseCanvasRPG.enabled = false;
            Time.timeScale = 1f;
        }
        else
        {
            pauseCanvasRPG.enabled = true;
            Time.timeScale = 0f;
        }
    }

    public void OnMenuButtonClick()
    {

    }

    public void OnResumeButton()
    {
        pauseCanvasRPG.enabled = false;
        Time.timeScale = 1f;
    }

    public void OnQuitButtonClick()
    {

    }
}
