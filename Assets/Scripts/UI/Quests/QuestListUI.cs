using System.Linq;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestTooltipUI questTooltip;
        [SerializeField] Transform listRoot;
        [SerializeField] QuestItemUI questPrefab;
        QuestList questList;

        void Start()
        {
            questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();

            foreach (QuestFilterUI questFilterUI in GetComponentsInChildren<QuestFilterUI>())
            {
                questFilterUI.Setup(questList);
            }

            questList.onListUpdated += Redraw;

            Redraw();
        }

        void Redraw()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (QuestStatus status in questList.GetFilteredStatuses().Reverse())
            {
                QuestItemUI questInstance = Instantiate<QuestItemUI>(questPrefab, listRoot);
                questInstance.Setup(status, questTooltip);
            }

            foreach (QuestFilterUI questFilterUI in GetComponentsInChildren<QuestFilterUI>())
            {
                questFilterUI.Redraw();
            }
   
            if(questList.GetFilteredStatuses().Count() > 0)
            {
                questTooltip.Setup(questList.GetFilteredStatuses().Last());
            }
            else
            {
                questTooltip.Setup(null);
            }
        }
    }
}