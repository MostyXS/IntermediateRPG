using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 5f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onProjectileHit;
        [SerializeField] UnityEvent onProjectileLaunched;

        Health target = null;
        GameObject instigator = null;
        float damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
            onProjectileLaunched.Invoke();
        }
        public void SetTarget(Health target, float damage, GameObject instigator)
        {
            this.instigator = instigator;
            this.target = target;
            this.damage = damage;
            Destroy(gameObject, maxLifeTime);
        }

        private void Update()
        {
            if (!target) return;

            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);


        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (!targetCapsule) return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target || target.IsDead()) return;
            onProjectileHit.Invoke();
            target.TakeDamage(instigator,damage);  
            if (hitEffect)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            for (int i = 0; i < destroyOnHit.Length; i++)
            {
                Destroy(destroyOnHit[i]);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
