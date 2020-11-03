using GameDevTV.Inventories;
using RPG.Attributes;
using UnityEngine;
namespace RPG.Combat
{
    

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem
    {
        [SerializeField] float damage = 5f;
        [Range(0,20)][SerializeField] float weaponPercentageBonus = 0;
        [SerializeField] float range = 2f;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equppedPrefab = null;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";
        

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;
            if (equppedPrefab != null)
            {
                Transform handTransform = GetHandTransform(rightHand, leftHand);

                weapon = Instantiate(equppedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (!oldWeapon)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (!oldWeapon) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);

        }


        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target,GameObject instigator, float calculatedDamage)
        {

            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target,calculatedDamage,instigator);
        }

        public float GetPercentageBonus()
        {
            return weaponPercentageBonus;
        }
        public float GetDamage()
        {
            return damage;
        }

        public float GetRange()
        {
            return range;
        }
        
        
    }
}