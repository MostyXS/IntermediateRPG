using GameDevTV.Utils;
using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        
        [Range(1, 99)][SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] ParticleSystem levelUpParticles = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        Experience experience;
        private void Awake()
        {
             experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }
        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (!experience) return;
            experience.onExperienceGained += UpdateLevel;
        }
        private void OnDisable()
        {
            if (!experience) return;
            experience.onExperienceGained -= UpdateLevel;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel>currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            if (!levelUpParticles) return;
            Instantiate(levelUpParticles, transform.position, Quaternion.identity, transform);
        }
        public int GetLevel()
        {         
            return currentLevel.value;
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1+GetPercentageModifier(stat));
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float totalPercentage = 0;
            IEnumerable providers = GetComponents<IModifierProvider>();

            foreach (IModifierProvider provider in providers)
            {
                foreach(float modifier in provider.GetPercentageModifiers(stat))
                {
                    totalPercentage += modifier;
                }
            }
            return totalPercentage;


        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            
            float total = 0;
            IEnumerable providers = GetComponents<IModifierProvider>();

            foreach (IModifierProvider provider in providers)
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
            
        }

        private int CalculateLevel()
        { 
            Experience experience = GetComponent<Experience>();
            if (!experience) return startingLevel;

            float currentXP = GetComponent<Experience>().GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (xpToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }

    }
}
