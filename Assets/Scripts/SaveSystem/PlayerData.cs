using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string scene;
    public float[] position;
    public int[] completion;
    public float score;

    private PlayerController playerController;

    public PlayerData (StatsManager player)
    {

        scene = player.currentScene;

        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;

        for (int i = 0; i < player.completion.Length; i++)
        {
            completion[i] = player.completion[i];
        }

        score = 0; //replace with actual value once I know it
    }
}
