using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Utils;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, IModifierProvider, ISaveable, IPredicateEvaluator
    {
        [SerializeField] TraitBonus[] bonusConfig;
        Dictionary<Trait, int> assignedPoints = new();
        Dictionary<Trait, int> stagedPoints = new();
        Dictionary<Stat, Dictionary<Trait, float>> additiveBonusCache;
        Dictionary<Stat, Dictionary<Trait, float>> percentageBonusCache;
        bool activeEnhancer = false;
        public event Action storeUpdated;

        [System.Serializable]
        struct TraitBonus
        {
            public Trait trait;
            public Stat stat;
            public float additiveBonusPerPoint;
            public float percentageBonusPerPoint;
        }

        public static TraitStore GetPlayerTraitStore()
        {
            return GameObject.FindWithTag("Player").GetComponent<TraitStore>();
        }

        public bool ActiveEnhancer()
        {
            return activeEnhancer;
        }

        public int GetUnassignedPoints()
        {
            return GetAssignablePoints() - GetTotalProposedPoints();
        }

        public int GetTotalProposedPoints()
        {
            int total = 0;

            foreach(int points in assignedPoints.Values)
            {
                total += points;
            }

            return total + GetTotalStagedPoints();
        }

        public int GetTotalStagedPoints()
        {
            int total = 0;

            foreach(int points in stagedPoints.Values)
            {
                total += points;
            }

            return total;
        }

        public int GetProposedPoints(Trait trait)
        {
            return GetPoints(trait) + GetStagedPoints(trait);
        }

        public int GetPoints(Trait trait)
        {
            return assignedPoints.ContainsKey(trait) ? assignedPoints[trait] : 0;
        }

        public int GetStagedPoints(Trait trait)
        {
            return stagedPoints.ContainsKey(trait) ? stagedPoints[trait] : 0;
        }

        public void SetActiveEnhancer(bool activeEnhancer)
        {
            this.activeEnhancer = activeEnhancer;
            storeUpdated?.Invoke();
        }

        public void AssignPoints(Trait trait, int points)
        {
            if(CanAsignPoints(trait, points))
            {
                stagedPoints[trait] = GetStagedPoints(trait) + points;
                storeUpdated?.Invoke();
            }
        }

        public bool CanAsignPoints(Trait trait, int points)
        {
            if(GetStagedPoints(trait) + points < 0) 
            {
                return false;
            }

            if(GetUnassignedPoints() < points) 
            {
                return false;
            }

            return true;
        }

        public void Commit()
        {
            foreach(Trait trait in stagedPoints.Keys)
            {
                assignedPoints[trait] = GetProposedPoints(trait);
            }

            stagedPoints.Clear();
            storeUpdated?.Invoke();
        }

        public int GetAssignablePoints()
        {
            return (int)GetComponent<BaseStats>().GetStat(Stat.TotalTraitPoints);
        }

        void Awake()
        {
            additiveBonusCache = new();
            percentageBonusCache = new();

            foreach(var bonus in bonusConfig)
            {
                if(!additiveBonusCache.ContainsKey(bonus.stat))
                {
                    additiveBonusCache[bonus.stat] = new Dictionary<Trait, float>();
                }

                if(!percentageBonusCache.ContainsKey(bonus.stat))
                {
                    percentageBonusCache[bonus.stat] = new Dictionary<Trait, float>();
                }

                additiveBonusCache[bonus.stat][bonus.trait] = bonus.additiveBonusPerPoint;
                percentageBonusCache[bonus.stat][bonus.trait] = bonus.percentageBonusPerPoint;
            }
        }

        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(Stat stat)
        {
            if(!additiveBonusCache.ContainsKey(stat)) 
            {
                yield break;
            }

            foreach(Trait trait in additiveBonusCache[stat].Keys)
            {
                float bonus = additiveBonusCache[stat][trait];

                yield return bonus * GetPoints(trait);
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(Stat stat)
        {
            if(!percentageBonusCache.ContainsKey(stat)) 
            {
                yield break;
            }

            foreach(Trait trait in percentageBonusCache[stat].Keys)
            {
                float bonus = percentageBonusCache[stat][trait];

                yield return bonus * GetPoints(trait);
            }
        }

        object ISaveable.CaptureState()
        {
            return assignedPoints;
        }

        void ISaveable.RestoreState(object state)
        {
            assignedPoints = new Dictionary<Trait, int>((Dictionary<Trait, int>)state);
        }

        bool? IPredicateEvaluator.Evaluate(EPredicate predicate, string[] parameters)
        {
            switch(predicate)
            {
                case EPredicate.MinimumTrait:

                    if(Enum.TryParse(parameters[0], out Trait trait))
                    {
                        return GetPoints(trait) >= int.Parse(parameters[1]);
                    } 

                    break;

                case EPredicate.HasTraitPoints:
                
                    return GetUnassignedPoints() > 0;
            }

            return null;
        }
    }
}
