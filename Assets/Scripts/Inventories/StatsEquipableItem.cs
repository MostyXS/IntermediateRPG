using GameDevTV.Inventories;
using RPG.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventory/Equipable Item")]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField]
        Modifier[] additiveModifiers;
        [SerializeField]
        Modifier[] percentageModifiers;

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {

            foreach(Modifier additiveModifier in additiveModifiers)
            {
                if(additiveModifier.stat == stat)
                {
                    yield return additiveModifier.value;
                }
            }
            yield return 0;

        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            
            yield return 0;
        }

        [Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }
        
    }
}