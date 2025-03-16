using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(SphereCollider))]
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0;
        [SerializeField] float respawnTime = 5f;

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                Pickup(other.gameObject);
            }
        }

        void Pickup(GameObject subject)
        {
            if(weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }

            if(healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }

            StartCoroutine(HideForSeconds(respawnTime));
        }

        IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);

            yield return new WaitForSeconds(seconds);

            ShowPickup(true);
        }

        void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;

            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        bool IRaycastable.HandleRaycast(PlayerController callingController)
        {           
            if(Input.GetKeyDown(callingController.GetInteractionKey()))
            {
                Pickup(callingController.gameObject);
            }

            return true;
        }

        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
