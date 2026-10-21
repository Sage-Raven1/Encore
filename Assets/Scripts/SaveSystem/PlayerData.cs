using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public string scene;
    public float[] position;
    public bool[] completion;

    private PlayerController playerController;

    public PlayerData (StatsManager player)
    {

        scene = player.currentScene;
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        for (int i = 0; i < player.completion.length; i++)
        {
            completion[i] = player.completion[i];
        }
    }
}
