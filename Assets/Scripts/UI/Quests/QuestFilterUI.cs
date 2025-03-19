using System;
using RPG.Quests;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Quests
{
    public class QuestFilterUI : MonoBehaviour
    {
        [SerializeField] QuestType questType;
        QuestList questList;
        Button button;

        public void Setup(QuestList questList)
        {
            this.questList = questList;
        }

        public void Redraw()
        {
            button.interactable = questList.GetFilter() != questType;
        }

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }

        void SelectFilter()
        {
            questList.SelectFilter(questType);
        }
    }
}
