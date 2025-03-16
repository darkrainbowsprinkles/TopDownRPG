using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroupTrigger : MonoBehaviour
    {
        [SerializeField] Fighter[] fighters;

        void Trigger()
        {
            foreach(Fighter fighter in fighters)
            {
                if(fighter.TryGetComponent(out CombatTarget target))
                {
                    target.enabled = true;
                }

                fighter.enabled = true;
            }
        }
    }
}