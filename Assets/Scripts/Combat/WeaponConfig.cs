using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(menuName = "RPG/Inventory/New Weapon")]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] AnimatorOverrideController animatorOverride;
        [SerializeField] Weapon equippedWeaponPrefab;
        [SerializeField] Projectile projectile;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] bool isRightHanded = true;
        const string weaponName = "Weapon";
        
        public bool HasProjectile()
        {
            return projectile != null;
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {   
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if(equippedWeaponPrefab != null)
            {
                Transform handTransform = GetHandTransfrom(rightHand, leftHand);
                weapon = Instantiate(equippedWeaponPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var defaultController = GetDefaultController(animator);

            if(animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if(defaultController != null)
            {
                animator.runtimeAnimatorController = defaultController.runtimeAnimatorController;
            }

            return weapon;
        }

        public void LaunchProjectile(GameObject instigator, Transform rightHand, Transform leftHand, Health target, float calculatedDamage)
        {
            Transform handTransform = GetHandTransfrom(rightHand, leftHand);
            Projectile projectileInstance = Instantiate(projectile, handTransform.position, Quaternion.identity);
            projectileInstance.SetProjectileInfo(target, instigator, calculatedDamage);
        }

        Transform GetHandTransfrom(Transform rightHand, Transform leftHand)
        {
            if(isRightHanded) 
            {
                return rightHand;
            }
            else 
            {
                return leftHand;
            }
        }

        void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {   
            Transform oldWeapon = rightHand.Find(weaponName);

            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }

            if(oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        AnimatorOverrideController GetDefaultController(Animator animator)
        {
            return animator.runtimeAnimatorController as AnimatorOverrideController;
        }

        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return weaponDamage;
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }
    }
}
