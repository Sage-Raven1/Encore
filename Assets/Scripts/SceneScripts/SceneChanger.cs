using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    //public Animator fadeOut;
    public float fadeTime = 5f;
    public Vector2 newPlayerLocation;

    private Transform _player;
    private StatsManager statsManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            statsManager = GetComponent<StatsManager>();
            _player = collision.transform;
            //fadeOut.Play("make an animation first ;-;");
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
        statsManager.currentScene = sceneToLoad;
        _player.position = newPlayerLocation;
    }

}
