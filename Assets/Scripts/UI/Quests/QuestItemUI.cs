using RPG.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI progress;
        QuestTooltipUI questTooltip;
        QuestStatus status;

        public void Setup(QuestStatus status, QuestTooltipUI questTooltip)
        {
            this.status = status;
            this.questTooltip = questTooltip;
            SetText();
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => questTooltip.Setup(status));
        }

        void SetText()
        {
            Color textColor = status.IsComplete() ? Color.gray : title.color;
            title.color = textColor;
            progress.color = textColor;
            title.text = status.GetQuest().GetTitle();
            progress.text = status.GetCompletedCount() + "/" + status.GetQuest().GetObjectiveCount();
        }
    }
}