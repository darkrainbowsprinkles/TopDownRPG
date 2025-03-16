using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Utils.UI
{
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] GameObject tooltipPrefab;
        GameObject tooltip;

        void OnDestroy()
        {
            ClearTooltip();
        }

        void OnDisable()
        {
            ClearTooltip();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            var parentCanvas = GetComponentInParent<Canvas>();

            if(tooltip && !CanCreateTooltip())
            {
                ClearTooltip();
            }

            if(!tooltip && CanCreateTooltip())
            {
                tooltip = Instantiate(tooltipPrefab, parentCanvas.transform);
            }

            if(tooltip)
            {
                UpdateTooltip(tooltip);
                PositionTooltip();
            }
        }

        void PositionTooltip()
        {
            Canvas.ForceUpdateCanvases();

            var tooltipCorners = new Vector3[4];
            tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);

            var slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            bool below = transform.position.y > Screen.height / 2;
            bool right = transform.position.x < Screen.width / 2;

            int slotCorner = GetCornerIndex(below, right);
            int tooltipCorner = GetCornerIndex(!below, !right);

            tooltip.transform.position = slotCorners[slotCorner] - tooltipCorners[tooltipCorner] + tooltip.transform.position;
        }

        int GetCornerIndex(bool below, bool right)
        {
            if(below && !right) 
            {
                return 0;
            }
            else if(!below && !right) 
            {
                return 1;
            }
            else if(!below && right) 
            {
                return 2;
            }
            else 
            {
                return 3;
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            var slotCorners = new Vector3[4];

            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            Rect rect = new(slotCorners[0], slotCorners[2] - slotCorners[0]);

            if(!rect.Contains(eventData.position)) 
            {
                ClearTooltip();
            }

        }

        void ClearTooltip()
        {
            if(tooltip != null)
            {
                Destroy(tooltip);
            }
        }

        public abstract void UpdateTooltip(GameObject tooltip);
        public abstract bool CanCreateTooltip();
    }
}