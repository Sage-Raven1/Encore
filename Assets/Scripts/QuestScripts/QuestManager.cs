using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private Dictionary<QuestSo, Dictionary<QuestObjective, int>> questProgress = new();
    
    public List<QuestSo> GetActiveQuests()
    {
        return new List<QuestSo>(questProgress.Keys);
    }

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
    }

    public void AcceptQuest(QuestSo questSo)
    {
        questProgress[questSo] = new Dictionary<QuestObjective, int>();

        foreach (var objective in questSo.objectives)
        {
            UpdateObjectiveProgress(questSo, objective);
        }
        QuestLogUI.Instance.RefreshQuestList();
    }


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
            newAmount = InventoryManager.Instance.GetItemQuantity(objective.targetItem);
        }
        else if (objective.targetNpc != null && DialogueHistoryTracker.Instance.HasSpokenWith(objective.targetNpc))
        {
            newAmount = objective.requiredAmount;
        }

        progressDictionary[objective] = newAmount;
    }

    public string GetProgressText(QuestSo questSo, QuestObjective objective)
    {
        int currentAmount = GetCurrentAmount(questSo, objective);

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

    public int GetCurrentAmount(QuestSo questSo, QuestObjective objective)
    {
        if (questProgress.TryGetValue(questSo, out var objectiveDictionary))
        {
            if (objectiveDictionary.TryGetValue(objective, out int amount))
            {
                return amount;
            }
        }
        return 0;
    }
}
