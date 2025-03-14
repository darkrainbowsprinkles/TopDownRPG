using UnityEngine;
using GameDevTV.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;
using UnityEngine.AI;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable, IAttributeProvider
    {
        [SerializeField] float regenerationPercentage = 100f;
        [SerializeField] public UnityEvent<float> onDamageTaken;
        [SerializeField] public UnityEvent onDie;
        LazyValue<float> health;
        BaseStats baseStats;
        ActionScheduler actionScheduler;
        bool wasDeadLastFrame = false;

        public float GetCurrentValue()
        {
            return health.value;
        }

        public float GetMaxValue()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetFraction()
        {
            return health.value / baseStats.GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return health.value == 0;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if(IsDead())
            {
                return;
            }

            health.value = Mathf.Max(0f, health.value - damage);

            if(IsDead())
            {
                AwardExperience(instigator);
                Die();
            }
            else
            {
                onDamageTaken.Invoke(damage);
            }

            UpdateHealthState();
        }

        public void Heal(float amount)
        {
            health.value = Mathf.Min(health.value + amount, GetMaxValue());
            UpdateHealthState();
        }

        void Awake()
        {
            health = new LazyValue<float>(GetInitialHealth);
            baseStats = GetComponent<BaseStats>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        void Start()
        {
            health.ForceInit();
        }

        float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        void OnEnable()
        {
            baseStats.onLevelUp += RegenerateHealth;
        }

        void OnDisable()
        {
            baseStats.onLevelUp -= RegenerateHealth;
        }

        void UpdateHealthState()
        {
            Animator animator = GetComponent<Animator>();

            if(!wasDeadLastFrame && IsDead())
            {           
                actionScheduler.CancelCurrentAction();
                animator.SetTrigger("die");
            }

            if(wasDeadLastFrame && !IsDead())
            {
                animator.Rebind();
            }

            wasDeadLastFrame = IsDead();
        }

        void RegenerateHealth()
        {
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * (regenerationPercentage / 100);
            health.value = Mathf.Max(health.value, regenHealthPoints);
        }

        void AwardExperience(GameObject instigator)
        {
            if(instigator.TryGetComponent(out Experience experience)) 
            {
                experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
            }
        }

        void Die()
        {
            onDie.Invoke();
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }

        object ISaveable.CaptureState()
        {
            return health.value;
        }

        void ISaveable.RestoreState(object state)
        {
            health.value = (float) state;
            UpdateHealthState();
        }
    }
}
