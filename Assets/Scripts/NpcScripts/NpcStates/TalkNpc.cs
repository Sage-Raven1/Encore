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
        _body.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnDisable()
    {
        dialoguePrompt.enabled = false;
        _body.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
        for (int i = 0; 1 < conversations.Count; i++)
        {
            var convo = conversations[i];
            if (convo != null && convo.IsConditionMet())
            {
                conversations.RemoveAt(i);
                currentConversation = convo;
            }
        }
    }
}
