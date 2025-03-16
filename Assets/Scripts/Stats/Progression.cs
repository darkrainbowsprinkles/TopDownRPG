using UnityEngine;
using System.Collections.Generic;

namespace RPG.Stats
{
    [CreateAssetMenu(menuName = "RPG/Stats/New Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgessionCharacterClass[] characterClasses;
        Dictionary<CharacterClass, Dictionary<Stat, int[]>> statsLookup;

        [System.Serializable]
        struct ProgessionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stat;
        }

        [System.Serializable]
        struct ProgressionStat
        {
            public Stat stat;
            public int[] levels;
        }

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            if(!statsLookup[characterClass].ContainsKey(stat))
            {
                return 0;
            }

            int[] levels = statsLookup[characterClass][stat];

            if(levels.Length == 0)
            {
                return 0;
            }

            if(level > levels.Length)
            {
                return levels[levels.Length - 1];
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            if(!statsLookup[characterClass].ContainsKey(stat))
            {
                return 0;
            }
            
            int[] levels = statsLookup[characterClass][stat];

            return levels.Length;
        }

        void BuildLookup()
        {
            if(statsLookup != null) 
            {
                return;
            }

            statsLookup = new Dictionary<CharacterClass, Dictionary<Stat, int[]>>();

            foreach(ProgessionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, int[]>();

                foreach(ProgressionStat progressionStat in progressionClass.stat)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                statsLookup[progressionClass.characterClass] = statLookupTable;
            }
        }
    }
}

