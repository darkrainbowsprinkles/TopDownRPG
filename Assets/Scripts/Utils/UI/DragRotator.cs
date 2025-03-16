using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Utils.UI
{
    public class DragRotator : MonoBehaviour, IDragHandler
    {
        [SerializeField] GameObject dragTarget;

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            dragTarget.transform.Rotate(Vector3.up, -eventData.delta.x, Space.World);
        }
    }
}
