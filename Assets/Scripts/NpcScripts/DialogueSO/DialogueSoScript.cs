using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Npc SO/NPC Dialogue")]
public class DialogueSoScript : ScriptableObject
{
    public DialogueLine[] dialogueLines;

    [Header("Conditional Dialogue Requirements (Optional)")]
    public ActorSoScript[] requiredNpcs;
    public ItemSo[] requiredItems;

    [Header("Control Flags")]
    public bool removeAfterPlay;
    public List<DialogueSoScript> removeDialogueOnPlay;

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
        
        if (requiredItems.Length > 0)
        {
            foreach (var item in requiredItems)
            {
                if (!StatsManager.Instance.playerInventory.Contains(item.itemName))
                {
                    return false;
                }
            }
        }

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