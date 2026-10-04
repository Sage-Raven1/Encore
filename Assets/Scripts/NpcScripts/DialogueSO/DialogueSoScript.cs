using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Npc SO/NPC Dialogue")]
public class DialogueSoScript : ScriptableObject
{
    public DialogueLine[] dialogueLines;
}

[System.Serializable]
public class  DialogueLine
{
    public ActorSoScript speaker;
    //if error, might be here
    [TextArea(3, 7)] public string text;
}