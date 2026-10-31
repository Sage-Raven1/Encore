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

    private float _saveMoveSpeed;

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
            StatsManager.Instance.moveSpeed = _saveMoveSpeed;
        }
    }

    public void ShowDialogue()
    {
        _saveMoveSpeed = StatsManager.Instance.moveSpeed;

        DialogueLine line = _currentDialogue.dialogueLines[_dialogueIndex];

        DialogueHistoryTracker.Instance.RecordNpcDialogue(line.speaker);

        actorName.text = line.speaker.actorName; //speaker
        dialogueText.text = line.text;
        dialogueCanvas.enabled = true;

        StatsManager.Instance.moveSpeed = 0;

        _dialogueIndex++;
    }

    private void EndDialogue()
    {
        _dialogueIndex = 0;
        isDialogueActive = false;
        dialogueCanvas.enabled = false;
        dialoguePrompt.enabled = true;
        StatsManager.Instance.moveSpeed = _saveMoveSpeed;
    }
}
