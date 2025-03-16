using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventories/Stats Equipable Item")]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] Modifier[] additiveModifiers;
        [SerializeField] Modifier[] percentageModifiers;
        
        [System.Serializable]
        public struct Modifier
        {
            public Stat stat;
            public float value;
        }

        public IEnumerable<Modifier> GetAdditiveModifiers()
        {
            return additiveModifiers;
        }

        public IEnumerable<Modifier> GetPercentageModifiers()
        {
            return percentageModifiers;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {   
            foreach(Modifier modifier in additiveModifiers)
            {
                if(modifier.stat != stat) continue;

                yield return modifier.value;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach(Modifier modifier in percentageModifiers)
            {
                if(modifier.stat != stat) continue;

                yield return modifier.value;
            }
        }
    }
}
