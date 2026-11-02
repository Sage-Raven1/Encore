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
    public string[] playerInventory;
    public string currentScene = "MenuScreen";
    public int[] completion;
    public float score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void  LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        currentScene = data.scene;
        score = data.score;

        /*for (int i = 0; i < data.completion.Length; i++)
        {
            completion[i] = data.completion[i];
        }*/
    }
}
