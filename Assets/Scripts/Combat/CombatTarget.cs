using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(!callingController.GetComponent<Fighter>().CanAttack(gameObject)) 
            {
                return false;
            }

            if(Input.GetKeyDown(callingController.GetInteractionKey()))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }

            return true;
        }
    }
}

