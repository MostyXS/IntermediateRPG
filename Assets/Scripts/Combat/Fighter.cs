using RPG.Core;
using RPG.Movement;
using GameDevTV.Saving;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine.Events;
using GameDevTV.Inventories;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        
        
        [SerializeField] float timeBetweenAttacks = 3f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;


        Health target;
        Mover mover;
        Animator animator;
        Equipment equipment;


        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }

        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (!weapon)
            {
                EquipWeapon(defaultWeapon);

            }
            else
                EquipWeapon(weapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }
        Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }
        Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }
        public Health GetTarget() 
        {
            return target;
        }
        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig=weapon;
            currentWeapon.value = AttachWeapon(weapon);

        }
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target && !target.IsDead())
            {
                ProcessAttack();
            }
        }

        public void Cancel()
        {
            StopAttack();
            GetComponent<Mover>().Cancel();
            target = null;
        }
        public bool CanAttack(GameObject combatTarget)
        {
            if (!mover.CanMoveTo(combatTarget.transform.position)
                && !GetIsInRange(combatTarget.transform)) return false;

            return combatTarget && !combatTarget.GetComponent<Health>().IsDead();
        }
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }
        private void ProcessAttack()
        {
            if (GetIsInRange(target.transform))
            {
                mover.Cancel();
                AttackBehaviour();
            }
            else
            {                   
                mover.MoveTo(target.transform.position,1f);
            }
        }
        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("StopAttack");
            GetComponent<Animator>().SetTrigger("Attack");
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= currentWeaponConfig.GetRange();
        }


        

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
        }

        //Animation Events
        void Hit()
        {
            if (!target) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.GetComponent<Health>().TakeDamage(gameObject, damage);
            }  
        }

        void Shoot()
        {
            Hit();
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>((string)state);
            EquipWeapon(weapon);

        }
    }
}
