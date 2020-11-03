using GameDevTV.Inventories;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far the pickups be scattered from the dropper.")]
        [SerializeField] float scatterDistance = 1f;
        [SerializeField] DropLibrary dropLibrary;
        [SerializeField] int minDrops = 0, maxDrops = 2;

        const int ATTEMPS = 30;

        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();
            int quantity = Random.Range(minDrops, maxDrops);

            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }
        }
        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPS; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, .1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;

        }

    }
}
