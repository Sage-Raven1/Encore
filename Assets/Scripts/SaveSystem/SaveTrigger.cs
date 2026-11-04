using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : MonoBehaviour
{
    private StatsManager statsManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            statsManager = collision.GetComponent<StatsManager>();
            statsManager.SavePlayer();
        }
    }
}
