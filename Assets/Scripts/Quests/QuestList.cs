using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;
using RPG.Saving;
using RPG.Utils;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        List<QuestStatus> statuses = new();
        QuestType filter = QuestType.None;
        public event Action onListUpdated;

        public void AddQuest(Quest quest)
        {
            if(!HasQuest(quest)) 
            {
                QuestStatus newStatus = new QuestStatus(quest);
                statuses.Add(newStatus);
                onListUpdated?.Invoke();
            }
        }

        public void SelectFilter(QuestType filter)
        {
            this.filter = filter;
            onListUpdated?.Invoke();
        }

        public IEnumerable<QuestStatus> GetFilteredStatuses()
        {
            foreach(var status in statuses)
            {
                if(filter == QuestType.None || status.GetQuest().GetQuestType() == filter)
                {
                    yield return status;
                }
            }
        }

        public QuestType GetFilter()
        {
            return filter;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);

            if(status != null)
            {
                status.CompleteObjective(objective);
                
                if(status.IsComplete())
                {
                    GiveReward(quest);
                }

                onListUpdated?.Invoke();
            }
        }

        void Update()
        {
            CompleteObjectivesByPredicates();
        }

        void CompleteObjectivesByPredicates()
        {
            foreach(QuestStatus status in statuses)
            {
                if(status.IsComplete()) 
                {
                    continue;
                }

                Quest quest = status.GetQuest();

                foreach(var objective in quest.GetObjectives())
                {
                    if(status.IsObjectiveComplete(objective.reference))
                    {
                        continue;
                    }

                    if(!objective.usesCondition) 
                    {
                        continue;
                    }

                    if(objective.completionCondition.Check(GetComponents<IPredicateEvaluator>()))
                    {
                        CompleteObjective(quest, objective.reference);
                    }
                }   
            }
        }

        QuestStatus GetQuestStatus(Quest quest)
        {
            foreach(QuestStatus status in statuses)
            {
                if(status.GetQuest() == quest)
                {
                    return status;
                }
            }

            return null;
        }

        void GiveReward(Quest quest)
        {
            foreach(var reward in quest.GetRewards())
            {        
                bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);

                if(!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
            }
        }

        bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        object ISaveable.CaptureState()
        {
            List<object> state = new();

            foreach(QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        void ISaveable.RestoreState(object state)
        {
            List<object> stateList = state as List<object>;

            if(stateList == null) 
            {
                return;
            }

            statuses.Clear();

            foreach(object objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }

            onListUpdated?.Invoke();
        }

        bool? IPredicateEvaluator.Evaluate(EPredicate predicate, string[] parameters)
        {
            if(parameters.Length == 0) 
            {
                return null;
            }

            Quest quest = Quest.GetByName(parameters[0]);
            
            QuestStatus status = GetQuestStatus(quest);

            switch(predicate)
            {
                case EPredicate.HasQuest:

                    return HasQuest(quest);

                case EPredicate.CompletedQuest:

                    if(status != null)
                    {
                        return status.IsComplete();
                    }

                    return false;

                case EPredicate.CompletedObjective:       

                    if(status != null)
                    {
                        bool result = true;

                        for(int i = 1; i < parameters.Length; i++)
                        {
                            result = result && status.IsObjectiveComplete(parameters[i]);
                        }

                        return result;
                    } 

                    return false;    
            }

            return null;
        }
    }
}