using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, ISaveable
    {
        LazyValue<float> mana;
        BaseStats baseStats;

        public float GetCurrentMana()
        {
            return mana.value;
        }

        public float GetMaxMana()
        {
            return baseStats.GetStat(Stat.Mana);
        }

        public float GetManaPercentage()
        {
            return GetCurrentMana() / GetMaxMana();
        }

        public bool UseMana(float manaToUse)
        {
            if(manaToUse > mana.value)
            {
                return false;
            }

            mana.value -= manaToUse;
            return true;
        }

        void Awake()
        {
            mana = new LazyValue<float>(GetMaxMana);
            baseStats = GetComponent<BaseStats>();
        }

        void Start()
        {
            mana.ForceInit();
        }

        void Update()
        {
            if(mana.value < GetMaxMana())
            {
                mana.value += Time.deltaTime * GetRegenRate();

                if(mana.value > GetMaxMana())
                {
                    mana.value = GetMaxMana();
                }
            }
        }

        float GetRegenRate()
        {
            return baseStats.GetStat(Stat.ManaRegenRate);
        }

        object ISaveable.CaptureState()
        {
            return mana.value;
        }

        void ISaveable.RestoreState(object state)
        {
            mana.value = (float) state;
        }
    }
}
