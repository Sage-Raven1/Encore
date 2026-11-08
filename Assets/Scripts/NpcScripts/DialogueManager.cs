using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

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
    private QuestManager questManager;
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
        questManager = FindObjectOfType<QuestManager>();
    }

    public void StartDialogue(DialogueSoScript dialogue)
    {
        _currentDialogue = dialogue;
        _dialogueIndex = 0;
        isDialogueActive = true;
        dialogueCanvas.enabled = true;
        _saveMoveSpeed = StatsManager.Instance.moveSpeed;

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
            if (_currentDialogue.offerQuestOnEnd != null)
            {
                Debug.Log("Quest Offered: " + _currentDialogue.offerQuestOnEnd.questName);
                questManager.AcceptQuest(_currentDialogue.offerQuestOnEnd);//here is quest acceptance
            }
            else if (_currentDialogue.turnInQuestOnEnd != null && _currentDialogue.turnInQuestOnEnd.objectives.All(obj => questManager.IsQuestCompleted(_currentDialogue.turnInQuestOnEnd, obj)))
            {
                Debug.Log("Quest Turned In: " + _currentDialogue.turnInQuestOnEnd.questName);
                questManager.CompleteQuest(_currentDialogue.turnInQuestOnEnd, _currentDialogue.turnInQuestOnEnd.objectives[0]);//here is quest turn in 
            }
            StatsManager.Instance.moveSpeed = _saveMoveSpeed;
            EndDialogue();
        }
    }

    public void ShowDialogue()
    {
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
    }
}
