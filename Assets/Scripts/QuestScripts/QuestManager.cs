using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSo, Dictionary<QuestObjective, int>> questProgress = new();

    public void UpdateObjectiveProgress(QuestSo questSo, QuestObjective objective)
    {
        if (!questProgress.ContainsKey(questSo))
        {
            questProgress[questSo] = new Dictionary<QuestObjective, int>();
        }
        var progressDictionary = questProgress[questSo];

        int newAmount = 0;

        if (objective.targetItem != null)
        {
            // Here you would implement logic to count the number of target items the player has collected.
        }
    }

    public string GetProgressText(QuestSo questSo, QuestObjective objective)
    {
        int currentAmount = 0;

        if (currentAmount >= objective.requiredAmount)
        {
            return "Complete";
        }
        else if (objective.targetItem != null)
        {
            return $"{currentAmount}/{objective.requiredAmount} {objective.targetItem.itemName}";
        }
        else
        {
            return "In Progress";
        }
    }
}
