using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventories/Drop Library")]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] DropConfig[] potentialDrops;
        [SerializeField] float[] dropChancePercentage;
        [SerializeField] int[] minDrops;
        [SerializeField] int[] maxDrops;


        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;

            public int GetRandomNumber(int level)
            {
                if(!item.IsStackable())
                {
                    return 1;
                }
                
                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);

                return Random.Range(min, max + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;

            public Dropped(InventoryItem item, int number)
            {
                this.item = item;
                this.number = number;
            }
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if(values.Length == 0) 
            {
                return default;
            }
            if(level > values.Length)
            {
                return values[values.Length - 1];
            }
            if(level <= 0)
            {
                return default;
            }

            return values[level -1];
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if(!ShouldRandomDrop(level))
            {
                yield break;
            }

            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        bool ShouldRandomDrop(int level)
        {
            return Random.Range(0,100) < GetByLevel(dropChancePercentage, level);
        }

        int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(minDrops, level);
            int max = GetByLevel(maxDrops, level);

            return Random.Range(min, max);
        }

        Dropped GetRandomDrop(int level)
        {
            var drop = SelectRandomItem(level);

            return new Dropped(drop.item, drop.GetRandomNumber(level));
        }

        DropConfig SelectRandomItem(int level)
        {
            float randomRoll = Random.Range(0, GetTotalChance(level));
            float chanceTotal = 0;

            foreach(var drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance, level);

                if(chanceTotal > randomRoll)
                {
                    return drop;
                }
            }

            return null;
        }

        float GetTotalChance(int level)
        {
            float total = 0;

            foreach(var drop in potentialDrops)
            {
                total = GetByLevel(drop.relativeChance, level);
            }

            return total;
        }
    }
}