using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using RPG.Attributes;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime =5f, minWaypointDwellTime =0.5f, maxWaypointDwellTime=3f, aggroCooldownTime = 5f;
        [SerializeField] PatrolPath patrolPath = null;
        [Range(0,1f)][SerializeField] float patrolSpeedFraction = 0.2f; //Used when detects player
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float shoutDistance = 5f;
         
        GameObject player;
        Fighter myFighter;
        
        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity, timeSinceArrivedAtWaypoint = Mathf.Infinity, waypointDwellTime=1f;
        float timeSinceAggrevated = Mathf.Infinity;
        
        int currentWaypointIndex = 0;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            myFighter = GetComponent<Fighter>();
            guardPosition = new LazyValue<Vector3>(GetInitialPosition);
        }
        public Vector3 GetInitialPosition()
        {
            return transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private void Update()
        { 
            if (GetComponent<Health>().IsDead()) return;
                
            ProcessControl();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }
        private void ProcessControl()
        {
            if (IsAggrevated() && myFighter.CanAttack(player))
            {
                
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            Timers();
        }

        private void Timers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        { 
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                else
                {
                    nextPosition = GetCurrentWaypoint();
                }
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                waypointDwellTime = UnityEngine.Random.Range(minWaypointDwellTime, maxWaypointDwellTime);
                GetComponent<Mover>().StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.transform.GetChild(currentWaypointIndex).position;
        }
        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint <= waypointTolerance;

        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            myFighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up);
            for (int i =0; i<hits.Length; i++)
            {
                if (!hits[i].transform.GetComponent<AIController>()) continue;
                    hits[i].transform.GetComponent<AIController>().Aggrevate();
                
            }
        }

        private bool IsAggrevated()
        {

            return Vector3.Distance(player.transform.position, transform.position) <= chaseDistance 
                || timeSinceAggrevated < aggroCooldownTime;
        }
    }
}