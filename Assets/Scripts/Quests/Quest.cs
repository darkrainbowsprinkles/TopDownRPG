using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(menuName = "RPG/Quests/New Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] QuestType questType;
        [SerializeField] [TextArea] string description = "";
        [SerializeField] List<Objective> objectives = new();
        [SerializeField] List<Reward> rewards = new();

        [System.Serializable]
        public class Objective
        {
            public string reference;
            [TextArea] public string description;
            public bool usesCondition = false;
            public Condition completionCondition;
        }

        [System.Serializable]
        public class Reward
        {         
            [Min(1)] public int number;
            public InventoryItem item;
        }

        public static Quest GetByName(string questName)
        {
            foreach(Quest quest in Resources.LoadAll<Quest>(""))
            {
                if(questName == quest.name)
                {
                    return quest;
                }
            }

            return null;
        }

        public QuestType GetQuestType()
        {
            return questType;
        }

        public string GetDescription()
        {
            return description;
        }

        public string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public bool HasObjective(string objectiveReference)
        {
            foreach(Objective objective in objectives)
            {
                if(objective.reference == objectiveReference)
                {
                    return true;
                }
            }

            return false;
        }
    }
}