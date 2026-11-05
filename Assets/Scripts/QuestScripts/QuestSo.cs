using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "New Quest")]
public class QuestSo : ScriptableObject
{
    public string questName;

    public List<QuestObjective> objectives;
}

[System.Serializable]
public class QuestObjective
{
    public string description;

    [SerializeField] private Object target;

    public ItemSo targetItem => target as ItemSo;
    public ActorSoScript targetNpc => target as ActorSoScript;

    public int requiredAmount;
}