using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalkNpc : MonoBehaviour
{
    private Rigidbody2D _body;

    public TMP_Text dialoguePrompt;

    public DialogueSoScript currentConversation;
    public List<DialogueSoScript> conversations;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        dialoguePrompt.enabled = false;
    }

    private void OnEnable()
    {
        dialoguePrompt.enabled = true;
        _body.velocity = Vector2.zero;
        _body.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnDisable()
    {
        if (dialoguePrompt != null)
        {
            dialoguePrompt.enabled = false;
        }
        _body.velocity = Vector2.zero;
        _body.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && InventoryManager.Instance.inventoryCanvas.enabled == false && QuestLogUI.Instance.questCanvas.enabled == false)
        {
            //Debug.Log("pressed");
            if (DialogueManager.Instance.isDialogueActive)
            {
                DialogueManager.Instance.AdvanceDialogue();
            }
            else
            {
                CheckForNewConversation();
                DialogueManager.Instance.StartDialogue(currentConversation);
            }
        }
    }

    private void CheckForNewConversation()
    {
        for (int i = 0; i < conversations.Count; i++)
        {
            var convo = conversations[i];
            if (convo != null && convo.IsConditionMet())
            {
                currentConversation = convo;

                if(convo.removeAfterPlay || convo.offerQuestOnEnd != null)
                {
                    conversations.RemoveAt(i);
                }

                if (convo.removeDialogueOnPlay != null && convo.removeDialogueOnPlay.Count > 0)
                {
                    foreach (var dialogueToRemove in convo.removeDialogueOnPlay)
                    {
                        conversations.Remove(dialogueToRemove);
                    }
                }
                break;
            }
        }
    }
}
