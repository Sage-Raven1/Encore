using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("Player Stats")]
    public float moveSpeed = 5;
    public List<string> playerInventory = new List<string>();
    public string currentScene = "MenuScreen";
    public int[] completion;
    public float score;
    private Rigidbody2D player;
    public Vector2 position2;
    public float[] position = {0f, 0f};


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            player = GetComponent<Rigidbody2D>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayer()
    {
        /*var body = player.transform.position;
        position[0] = body.x;
        position[1] = body.y;*/
        SaveSystem.SavePlayer(this);
    }

    public void  LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        /*var body = player.transform.position;

        body.x = data.position[0];
        body.y = data.position[1];*/

        currentScene = data.scene;
        score = data.score;

        /*for (int i = 0; i < data.completion.Length; i++)
        {
            completion[i] = data.completion[i];
        }*/
    }
}
