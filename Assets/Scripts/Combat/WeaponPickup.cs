using RPG.Attributes;
using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup :MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0;
        [SerializeField] float respawnTime = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag=="Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {   if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);

            }
            if (healthToRestore >0)
            {
                subject.GetComponent<Health>().RestoreHealth(healthToRestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds (float time)
        {
            PickupSetActive(false);
            yield return new WaitForSeconds(time);
            PickupSetActive(true);
        }

        private void PickupSetActive(bool value)
        {
            GetComponent<Collider>().enabled = value;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(value);
            }
            
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);
            }
            return true;

        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }

}
