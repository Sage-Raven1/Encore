using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("Player Stats")]
    public float moveSpeed = 7;
    public string[] playerInventory;
    public string currentScene;
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
        
        Vector2 position;
        position.x = data.position[0];
        position.y = data.position[1];

        for (int i = 0; i < data.completion.Length; i++)
        {
            completion[i] = data.completion[i];
        }
    }
}
