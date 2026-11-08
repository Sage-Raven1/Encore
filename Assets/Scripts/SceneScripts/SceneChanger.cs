using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeOut;
    public float fadeTime = 5f;
    public Vector2 newPlayerLocation;

    private Transform _player;
    private StatsManager statsManager;
    private MainMenu mainMenu;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("Player entered scene changer");

            statsManager = collision.GetComponent<StatsManager>();
            UnityEngine.Debug.Log(statsManager.currentScene);
            _player = collision.transform;
            fadeOut.Play("FadeToBlack");
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(fadeTime);
        statsManager.currentScene = sceneToLoad;
        statsManager.SavePlayer();
        UnityEngine.Debug.Log(statsManager.currentScene);
        SceneManager.LoadScene(sceneToLoad);
        _player.position = newPlayerLocation;
    }

}
