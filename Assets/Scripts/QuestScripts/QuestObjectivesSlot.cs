using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class QuestObjectivesSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text trackingText;

    public void RefreshObjectives(string progressText, bool isComplete)
    {
        trackingText.text = progressText;

        Color color = isComplete ? Color.gray : Color.white;
        objectiveText.color = color;
        trackingText.color = color;
    }
}
