using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [SerializeField] float dropDistance = 1;
        [SerializeField] DropLibrary dropLibrary;
        const int dropAttempts = 30;

        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();
            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());

            foreach(var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for(int i = 0; i < dropAttempts; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * dropDistance;

                if(NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }
    }
}
