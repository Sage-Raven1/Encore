using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Npc SO/NPC Dialogue")]
public class DialogueSoScript : ScriptableObject
{
    public DialogueLine[] dialogueLines;
    public DialogueOption[] dialogueOptions;

    [Header("Conditional Dialogue Requirements (Optional)")]
    public ActorSoScript[] requiredNpcs;

    //items
    //locations

    public bool IsConditionMet()
    {
        if (requiredNpcs.Length > 0)
        {
            foreach (var npc in requiredNpcs)
            {
                if (!DialogueHistoryTracker.Instance.HasSpokenWith(npc))
                {
                    return false;
                }
            }
        }
        //check for item
        //check for locations

        return true;
    }

}

[System.Serializable]
public class  DialogueLine
{
    public ActorSoScript speaker;
    [TextArea(3, 7)] public string text;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueSoScript nextDialogue;
}