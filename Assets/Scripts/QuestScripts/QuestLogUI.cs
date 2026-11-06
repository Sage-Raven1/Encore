using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private QuestObjectivesSlot[] objectiveSlots;
    public Canvas questCanvas;
    public Canvas inventoryCanvas;

    private QuestSo questSo;

    private void Awake()
    {
        questCanvas.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (inventoryCanvas.enabled == true)
            {
                inventoryCanvas.enabled = false;
                questCanvas.enabled = true;
            }
            else if (questCanvas.enabled == true)
            {
                questCanvas.enabled = false;
            }
            else
            {
                questCanvas.enabled = true;
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
                questManager.UpdateObjectiveProgress(questSo, objective);

                int currentAmount = questManager.GetCurrentAmount(questSo, objective);
                string progress = questManager.GetProgressText(questSo, objective);
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

}
