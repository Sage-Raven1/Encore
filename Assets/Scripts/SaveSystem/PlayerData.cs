using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string scene;
    public int[] completion;
    public float score;
    //public float[] position;

    //private PlayerController playerController;

    public PlayerData (StatsManager player)
    {

        scene = player.currentScene;

        for (int i = 0; i < player.completion.Length; i++)
        {
            completion[i] = player.completion[i];
        }

        //position = player.position;

        score = 0; //replace with actual value once I know it
    }
}
