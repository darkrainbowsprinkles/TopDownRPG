using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] TextMeshProUGUI objectives;
        [SerializeField] TextMeshProUGUI rewards;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] Transform rewardsContainer;
        [SerializeField] QuestRewardUI rewardPrefab;
        QuestStatus status;

        public void Setup(QuestStatus status)
        {   
            this.status = status;
            FillTitles();
            FillObjectives();
            FillRewards();
        }

        void FillTitles()
        {
            if(status == null)
            {
                title.text = "No quests available";
                description.text = "";
                objectives.text = "";
                rewards.text = "";
                return;
            }

            title.text = status.GetQuest().GetTitle();
            description.text = status.GetQuest().GetDescription();
            objectives.text = "Objectives";
            rewards.text = "Rewards";
        }

        void FillObjectives()
        {
            foreach(Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }

            if(status == null) 
            {
                return;
            }

            foreach(var objective in status.GetQuest().GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;

                if(status.IsObjectiveComplete(objective.reference))
                {
                    prefab = objectivePrefab;
                }

                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective.description;
            }
        }

        void FillRewards()
        {
            foreach(Transform child in rewardsContainer)
            {
                Destroy(child.gameObject);
            }

            if(status == null) 
            {
                return;
            }

            foreach(var reward in status.GetQuest().GetRewards())
            {
                var rewardInstance = Instantiate(rewardPrefab, rewardsContainer);
                rewardInstance.Setup(reward.item, reward.number);
            }
        }
    }   
}