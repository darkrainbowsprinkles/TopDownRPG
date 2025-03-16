using UnityEngine;
using RPG.Utils;

namespace RPG.Quests
{
    public class QuestProvider : MonoBehaviour, IActionPerfomer
    {
        QuestList questList;

        public void GiveQuest(string questName)
        {
            QuestList questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
            questList.AddQuest(Quest.GetByName(questName));
        }

        public void CompleteObjective(string questName, string objectiveID)
        {
            questList.CompleteObjective(Quest.GetByName(questName), objectiveID);
        }

        void Awake()
        {
            questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
        }

        void IActionPerfomer.PerformAction(EAction action, string[] parameters)
        {
            switch(action)
            {
                case EAction.GiveQuest:
                    GiveQuest(parameters[0]);
                    break;

                case EAction.CompleteObjetive:
                    CompleteObjective(parameters[0], parameters[1]);
                    break;
            }
        }
    }
}