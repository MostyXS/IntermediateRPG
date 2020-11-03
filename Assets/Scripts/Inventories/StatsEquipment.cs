using GameDevTV.Inventories;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;
                foreach (float additiveModifier in item.GetAdditiveModifiers(stat))
                {
                    yield return additiveModifier; 
                }
                
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;
                foreach (float percentageModifier in item.GetPercentageModifiers(stat))
                {
                    yield return percentageModifier/100f;
                }

            }
        }
    }
}