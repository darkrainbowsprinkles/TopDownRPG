using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using GameDevTV.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, ICancelable
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] float autoAttackRange = 4f;
        float timeSinceLastAttack = Mathf.Infinity;
        Health currentTarget;
        Mover mover;
        Animator animator;
        Equipment equipment;
        BaseStats baseStats;
        ActionScheduler actionScheduler;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            currentTarget = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) 
            {
                return false;
            }

            if(!mover.CanMoveTo(combatTarget.transform.position) && !InRange(combatTarget.transform))
            {
                return false;
            }
            
            if(combatTarget.GetComponent<Health>().IsDead())
            {
                return false;
            }

            return true;
        }

        public Health GetCurrentTarget()
        {
            return currentTarget;
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        public Transform GetHandTransform(bool isRightHanded)
        {
            if(isRightHanded)
            {
                return rightHandTransform;
            }
            else
            {
                return leftHandTransform;
            }
        }

        public void Cancel()
        {
            currentTarget = null; 
            mover.Cancel();
            animator.ResetTrigger("attack");
            animator.SetTrigger("cancelAttack");
        }

        void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            equipment = GetComponent<Equipment>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();

            if(equipment != null)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        void Start()
        {
            currentWeapon.ForceInit();
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if(currentTarget == null) 
            {
                return;
            }

            if(currentTarget.IsDead())
            {
                currentTarget = FindNewTargetInRange();

                if(currentTarget == null) 
                {
                    return;
                }
            }

            if(!InRange(currentTarget.transform)) 
            {
                mover.StartMoveAction(currentTarget.transform.position, 1f);
            }
            else 
            {
                mover.Cancel();  
                AttackBehaviour();
            }
        }

        Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;

            if(weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        bool InRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.position) < currentWeaponConfig.GetRange();
        } 

        void AttackBehaviour()
        {
            FaceTarget();

            if(timeSinceLastAttack >= timeBetweenAttacks) 
            {
                timeSinceLastAttack = 0f;
                animator.SetTrigger("attack");
                animator.ResetTrigger("cancelAttack");
            }
        }

        Health FindNewTargetInRange()
        {
            Health bestTarget = null;

            float bestDistance = Mathf.Infinity;

            foreach(Health candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(transform.position, candidate.transform.position);

                if(candidateDistance < bestDistance)
                {
                    bestTarget = candidate;
                    bestDistance = candidateDistance;
                }
            }

            return bestTarget;
        }

        IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up);

            foreach(RaycastHit hit in hits)
            {
                if(!hit.transform.TryGetComponent(out Health health)) 
                {
                    continue;
                }

                if(health.IsDead()) 
                {
                    continue;
                }
                
                if(health.gameObject == gameObject) 
                {
                    continue;
                }

                yield return health;
            }
        }

        void FaceTarget()
        {
            Vector3 lookPosition = currentTarget.transform.position - transform.position;

            lookPosition.y = 0f;

            if(lookPosition != Vector3.zero) 
            {
                transform.rotation = Quaternion.LookRotation(lookPosition);
            }
        }


        // Animation Event
        void Hit()
        {
            if(currentTarget == null) 
            {
                return;
            }

            float damage = baseStats.GetStat(Stat.Damage);
            
            if(currentTarget.TryGetComponent(out BaseStats targetBaseStats))
            {
                float defence = targetBaseStats.GetStat(Stat.Defence);
                damage /= 1 + defence / damage;
            }

            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if(currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(gameObject, rightHandTransform, leftHandTransform, currentTarget, damage);
            }
            else
            {
                currentTarget.TakeDamage(gameObject, damage);
            }
        }
    }
}