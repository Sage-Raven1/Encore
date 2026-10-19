using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHistoryTracker : MonoBehaviour
{
    public static DialogueHistoryTracker Instance;
    private readonly List<ActorSoScript> _spokenNPCs = new List<ActorSoScript>();

    private void Awake(){
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RecordNpcDialogue(ActorSoScript actorSO)
    {
        _spokenNPCs.Add(actorSO); 
        Debug.Log("spoken to an NPC " + actorSO.actorName);
    }

    public bool HasSpokenWith(ActorSoScript actorSO)
    {
        return _spokenNPCs.Contains(actorSO);
    }
}
