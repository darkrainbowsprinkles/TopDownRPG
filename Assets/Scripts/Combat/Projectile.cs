using GameDevTV.Inventories;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] float lifeAfterHit = 0.5f;
        [SerializeField] bool isHoming = false;
        [SerializeField] string displayName = "";
        [SerializeField] GameObject hitEffect;
        [SerializeField] UnityEvent onHit;
        Health target;
        Vector3 targetPoint;
        GameObject instigator;
        float damage;

        public string GetDisplayName()
        {
            return displayName;
        }

        public bool SetProjectileInfo(Health target, GameObject instigator, float damage)
        {
            return SetProjectileInfo(instigator, damage, target);
        }

        public bool SetProjectileInfo(Vector3 targetPoint, GameObject instigator, float damage)
        {
            return SetProjectileInfo(instigator, damage, null, targetPoint);
        }

        public bool SetProjectileInfo(GameObject instigator, float damage, Health target=null, Vector3 targetPoint=default)
        {
            this.target = target;
            this.targetPoint = targetPoint;
            this.instigator = instigator;
            this.damage = damage;

            Destroy(gameObject, maxLifeTime);

            return true;
        }

        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if(target != null && isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(CalculateMovement());
        }

        void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();

            if(target != null && health != target) 
            {
                return;
            }

            if(health == null || health.IsDead()) 
            {
                return;
            }

            if(other.gameObject == instigator) 
            {
                return;
            }

            health.TakeDamage(instigator, damage);

            speed = 0f;

            onHit.Invoke();

            if(hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            Destroy(gameObject, lifeAfterHit);
        }

        Vector3 GetAimLocation()
        {
            if(target == null)
            {
                return targetPoint;
            }

            return target.transform.position + Vector3.up * GetTargetCenter();
        }

        float GetTargetCenter()
        {
            return target.GetComponent<CapsuleCollider>().height / 2;
        }

        Vector3 CalculateMovement()
        {
            return speed * Time.deltaTime * Vector3.forward;
        }
    }
}