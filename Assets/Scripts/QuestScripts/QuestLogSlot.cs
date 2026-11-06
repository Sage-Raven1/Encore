using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestLogSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText;

    public QuestSo currentQuest;

    public QuestLogUI questLogUI;

    private void OnValidate()
    {
        if (currentQuest != null)
        {
            SetQuest(currentQuest);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetQuest(QuestSo questSo)
    {
        currentQuest = questSo;
        questNameText.text = questSo.questName;

        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        currentQuest = null;
        gameObject.SetActive(false);
    }

    public void OnSlotClicked()
    {
        questLogUI.HandleQuestClicked(currentQuest);
        Debug.Log("Clicked");
    }


}
