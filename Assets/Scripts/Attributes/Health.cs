using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using RPG.Stats;
using RPG.Core;
using RPG.Saving;
using RPG.Utils;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 100f;
        LazyValue<float> health;
        BaseStats baseStats;
        ActionScheduler actionScheduler;
        bool wasDeadLastFrame = false;
        public UnityEvent<float> onDamageTaken;
        public UnityEvent onDie;

        public float GetCurrentHealth()
        {
            return health.value;
        }

        public float GetMaxHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetHealthPercentage()
        {
            return GetCurrentHealth() / GetMaxHealth();
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
            health.value = Mathf.Min(health.value + amount, GetMaxHealth());
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
