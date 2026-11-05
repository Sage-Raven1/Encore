using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    public void HandleQuestClicked(QuestSo questSo)
    {
        Debug.Log($"===Quest clicked: {questSo.questName}===");
        foreach (var objective in questSo.objectives)
        {
            questManager.UpdateObjectiveProgress(questSo, objective);
            Debug.Log($"Objective {objective.description} - {questManager.GetProgressText(questSo, objective)}");
        }
    }

}
