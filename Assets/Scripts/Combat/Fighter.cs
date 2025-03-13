using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using GameDevTV.Inventories;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, ICancelable
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] float autoAttackRange = 4f;

        readonly int AttackHash = Animator.StringToHash("attack");
        readonly int CancelAttackHash = Animator.StringToHash("cancelAttack");

        float timeSinceLastAttack = Mathf.Infinity;

        Health target;
        Equipment equipment;

        WeaponConfig currentWeaponConfig;

        LazyValue<Weapon> currentWeapon;

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);

            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) return false;
            if(!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !IsInRange(combatTarget.transform)) return false;

            Health currentTarget = combatTarget.GetComponent<Health>();

            return currentTarget != null && !currentTarget.IsDead;
        }

        public Health GetTarget()
        {
            return target;
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
            target = null; 

            GetComponent<Mover>().Cancel();

            GetComponent<Animator>().ResetTrigger(AttackHash);
            GetComponent<Animator>().SetTrigger(CancelAttackHash);
        }

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;

            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            equipment = GetComponent<Equipment>();

            if(equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if(target == null) return;

            if(target.IsDead)
            {
                target = FindNewTargetInRange();
                if(target == null) return;
            }

            if(!IsInRange(target.transform)) 
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else 
            {
                GetComponent<Mover>().Cancel();  
                AttackBehaviour();
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void UpdateWeapon()
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

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
        }

        private bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetRange();
        } 

        private void AttackBehaviour()
        {
            FaceTarget();

            if(timeSinceLastAttack < timeBetweenAttacks) return;

            timeSinceLastAttack = 0f;

            GetComponent<Animator>().SetTrigger(AttackHash);
            GetComponent<Animator>().ResetTrigger(CancelAttackHash);
        }

        private Health FindNewTargetInRange()
        {
            Health best = null;
            float bestDistance = Mathf.Infinity;

            foreach(Health candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(transform.position, candidate.transform.position);

                if(candidateDistance < bestDistance)
                {
                    best = candidate;
                    bestDistance = candidateDistance;
                }
            }

            return best;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up);

            foreach(RaycastHit hit in hits)
            {
                Health health = hit.transform.GetComponent<Health>();

                if(health == null) continue;
                if(health.IsDead) continue;
                if(health.gameObject == gameObject) continue;

                yield return health;
            }
        }

        private void FaceTarget()
        {
            Vector3 lookPosition = target.transform.position - transform.position;

            lookPosition.y = 0f;

            if(lookPosition == Vector3.zero) return;

            transform.rotation = Quaternion.LookRotation(lookPosition);
        }


        // Animation Event
        private void Hit()
        {
            if(target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            
            BaseStats targetBaseStats = target.GetComponent<BaseStats>();

            if(targetBaseStats != null)
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
                currentWeaponConfig.LaunchProjectile(gameObject, rightHandTransform, leftHandTransform, target, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }
    }
}