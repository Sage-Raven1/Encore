using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class QuestLogUI : MonoBehaviour
{
    public static QuestLogUI Instance;

    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private QuestObjectivesSlot[] objectiveSlots;
    [SerializeField] private QuestLogSlot[] questSlots;

    public Canvas questCanvas;
    
    private float _saveMoveSpeed;
    private QuestSo questSo;

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
        questCanvas.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !DialogueManager.Instance.isDialogueActive)
        {
            if (InventoryManager.Instance.inventoryCanvas.enabled == true)
            {
                InventoryManager.Instance.inventoryCanvas.enabled = false;
                questCanvas.enabled = true;
            }
            else if (questCanvas.enabled == true)
            {
                questCanvas.enabled = false;
                StatsManager.Instance.moveSpeed = _saveMoveSpeed;
            }
            else
            {
                _saveMoveSpeed = StatsManager.Instance.moveSpeed;
                questCanvas.enabled = true;
                StatsManager.Instance.moveSpeed = 0f;
            }
        }
    }

    public void HandleQuestClicked(QuestSo questSo)
    {
        this.questSo = questSo;

        questNameText.text = questSo.questName;

        DisplayObjectives();
    }

    private void DisplayObjectives()
    {
        for (int i = 0; i < objectiveSlots.Length; i++)
        {
            if (i < questSo.objectives.Count)
            {
                var objective = questSo.objectives[i];
                QuestManager.Instance.UpdateObjectiveProgress(questSo, objective);

                int currentAmount = QuestManager.Instance.GetCurrentAmount(questSo, objective);
                string progress = QuestManager.Instance.GetProgressText(questSo, objective);
                bool isComplete = currentAmount >= objective.requiredAmount;

                objectiveSlots[i].gameObject.SetActive(true);
                objectiveSlots[i].RefreshObjectives(progress, isComplete);
            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void RefreshQuestList()
    {
        List<QuestSo> activeQuests = QuestManager.Instance.GetActiveQuests();

        for (int i = 0; i < questSlots.Length; i++)
        {
            if (i < activeQuests.Count)
            {
                questSlots[i].SetQuest(activeQuests[i]);
            }
            else
            {
                questSlots[i].ClearSlot();
            }
        }
    }

}
