using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;
using System;
using UnityEngine.Events;
using RPG.Abilities;

namespace RPG.Control
{
    public class AIController : Controller
    {
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroCooldownTime = 4f;
        [SerializeField] float waypointTolerance = 2f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] float shoutDistance = 5f;
        [SerializeField] [Range(0f,1f)] float patrolSpeedFraction = 0.2f;
        [SerializeField] AbilitySequence[] abilitiesSequence;
        [SerializeField] public UnityEvent onAggrevated;
        [SerializeField] public UnityEvent onPacified;
        Fighter fighter;
        Mover mover;
        ActionScheduler scheduler;
        Health health;
        GameObject player;
        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        int currentWaypointIndex = 0;
        int currentAbilityIndex = 0;
        int currentAbilityUsage = 0;
        AbilityData[] currentAbilitySequence;

        [System.Serializable]
        struct AbilitySequence
        {
            [Range(0,1)] public float maxHealthFraction;
            [Range(0,1)] public float minHealthFraction;
            public AbilityData[] abilities;
        }

        [System.Serializable]
        struct AbilityData
        {
            public Ability ability;
            public int timesToUse;
        }

        public void Aggrevate()
        {
            onAggrevated?.Invoke();
            timeSinceAggrevated = 0f;
        }

        public void Reset()
        {
            mover.Teleport(guardPosition.value);
            ResetState();
        }

        public void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        public Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        void Awake()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
            guardPosition = new LazyValue<Vector3>(() => transform.position);
            guardPosition.ForceInit();
        }

        void Start()
        {
            SelectAbilitySequence(0);
        }

        void OnEnable()
        {
            health.onDamageTaken.AddListener(SelectAbilitySequence);
        }

        void OnDisable()
        {
            health.onDamageTaken.RemoveListener(SelectAbilitySequence);
        }

        void SelectAbilitySequence(float damage)
        {
            if(abilitiesSequence.Length == 0) 
            {
                return;
            }

            foreach(var sequence in abilitiesSequence)
            {
                if(sequence.abilities == currentAbilitySequence) 
                {
                    continue;
                }

                if(health.GetFraction() >= sequence.minHealthFraction && health.GetFraction() <= sequence.maxHealthFraction)
                {
                    mover.StartMoveAction(Vector3.zero, 0);
                    currentAbilitySequence = sequence.abilities;
                    currentAbilityUsage = 0;
                    currentAbilityIndex = 0;
                    return;
                }
            }
        }

        void Update()
        {
            if(health.IsDead()) 
            {
                return;
            }

            if(IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if(timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                onPacified?.Invoke();
                PatrolBehaviour();
            }
            
            UpdateTimers();
        }

        void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        void ResetState()
        {
            timeSinceLastSawPlayer = Mathf.Infinity;
            timeSinceArrivedWaypoint = Mathf.Infinity;
            timeSinceAggrevated = Mathf.Infinity;
            currentWaypointIndex = 0;
        }

        void AttackBehaviour()
        {
            if(!fighter.enabled) 
            {
                return;
            }

            if(abilitiesSequence.Length > 0)
            {
                UseAbilities();
            }
            else
            {
                fighter.Attack(player);
            }
            
            timeSinceLastSawPlayer = 0f;
            AggrevateNearbyEnemies();
        }

        void UseAbilities()
        {
            var selectedAbilityData = currentAbilitySequence[currentAbilityIndex];

            if(currentAbilityUsage < selectedAbilityData.timesToUse)
            {
                if(selectedAbilityData.ability.Use(gameObject))
                {
                    currentAbilityUsage++;
                }

                return;
            }
        
            currentAbilityUsage = 0;
            currentAbilityIndex = GetNextAbilityIndex();
        }

        int GetNextAbilityIndex()
        {
            if(currentAbilityIndex == currentAbilitySequence.Length - 1)
            {
                return 0;
            }

            return currentAbilityIndex + 1;
        }

        void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach(RaycastHit hit in hits)
            {
                AIController controller = hit.transform.GetComponent<AIController>();

                if(controller != null && !controller.GetComponent<Health>().IsDead())
                {
                    controller.Aggrevate();
                }
            }
        }

        void SuspicionBehaviour()
        {
            if(fighter.enabled)
            {
                scheduler.CancelCurrentAction();
            }
        }

        void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSinceArrivedWaypoint = 0f;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if(timeSinceArrivedWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }
        
        bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) <= waypointTolerance;
        }

        bool IsAggrevated()
        {
            return Vector3.Distance(transform.position, player.transform.position) <= chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }

        // Called in Unity
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}