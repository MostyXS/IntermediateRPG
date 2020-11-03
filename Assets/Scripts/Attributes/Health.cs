using GameDevTV.Saving;
using RPG.Stats;
using UnityEngine;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;
using System;

namespace RPG.Attributes
{

    public class Health : MonoBehaviour, ISaveable
    {
        [Range(0,1)][SerializeField] float regenerationPercentage = 0.7f;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }
        
        LazyValue<float> healthPoints;

        bool isDead;

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }
        private void Start()
        {
            healthPoints.ForceInit();
        }
        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
         
        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetFracton()
        {
            return GetHealthPoints() / GetMaxHealthPoints();
        }


        public bool IsDead()
        {
            return isDead;
        }
        public object CaptureState()
        {
            return healthPoints.value;
        }
        private void RegenerateHealth()
        {
            float regenHelathPoints = GetMaxHealthPoints() * regenerationPercentage;
            healthPoints.value = Mathf.Max(healthPoints.value, regenHelathPoints);
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if (healthPoints.value == 0)
            {
                Die();
            }
        }
        public void RestoreHealth(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value+healthToRestore, GetMaxHealthPoints());
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + "took damage: " + damage);
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            
            if (healthPoints.value == 0)
            {
                AwardExperience(instigator);
                Die();
            }
            else
            {
                takeDamage.Invoke(damage); 
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (!experience) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void Die()
        {
            if (!isDead)
            {
                onDie.Invoke();
                GetComponent<Animator>().SetTrigger("Death");
                isDead = true;
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }


    }
}