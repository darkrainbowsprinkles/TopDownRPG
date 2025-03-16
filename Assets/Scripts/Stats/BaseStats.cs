using System;
using UnityEngine;
using RPG.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField, Range(1, 99)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;
        Experience experience;
        LazyValue<int> currentLevel;
        public event Action onLevelUp;

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public CharacterClass GetCharacterClass()
        {
            return characterClass;
        }

        void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        void Start()
        {   
            currentLevel.ForceInit();
        }

        void OnEnable()
        {
            if(experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        void OnDisable()
        {
            if(experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        void UpdateLevel()
        {
            int newLevel = CalculateLevel();

            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp?.Invoke();
            }
        }

        void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        float GetAdditiveModifiers(Stat stat)
        {
            if(!shouldUseModifiers) 
            {
                return 0f;
            }

            float total = 0f;

            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }
        
        float GetPercentageModifier(Stat stat)
        {
            if(!shouldUseModifiers) 
            {
                return 0f;
            }

            float total = 0f;

            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        int CalculateLevel()
        {
            if(experience == null) 
            {
                return startingLevel;
            }

            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for(int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);

                if(XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        // float IAttributeProvider.GetMaxValue()
        // {
        //     return progression.GetLevels(Stat.ExperienceToLevelUp, characterClass) + 1;
        // }

        // float IAttributeProvider.GetCurrentValue()
        // {
        //     return GetLevel();
        // }
    }
}
