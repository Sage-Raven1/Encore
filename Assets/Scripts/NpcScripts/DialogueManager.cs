using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public Canvas dialogueCanvas;
    public TMP_Text actorName;
    public TMP_Text dialogueText;

    public TMP_Text dialoguePrompt;
    public bool isDialogueActive;

    private DialogueSoScript _currentDialogue;
    private int _dialogueIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        dialogueCanvas.enabled = false;
    }

    public void StartDialogue(DialogueSoScript dialogue)
    {
        _currentDialogue = dialogue;
        _dialogueIndex = 0;
        isDialogueActive = true;
        dialogueCanvas.enabled = true;

        ShowDialogue();
    }

    public void AdvanceDialogue()
    {
        if (_dialogueIndex < _currentDialogue.dialogueLines.Length)
        {
            ShowDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    public void ShowDialogue()
    {
        DialogueLine line = _currentDialogue.dialogueLines[_dialogueIndex];

        actorName.text = line.speaker.actorName; //speaker
        dialogueText.text = line.text;
        dialogueCanvas.enabled = true;

        _dialogueIndex++;
    }

    private void EndDialogue()
    {
        _dialogueIndex = 0;
        isDialogueActive = false;
        dialogueCanvas.enabled = false;
        dialoguePrompt.enabled = true;
    }
}
