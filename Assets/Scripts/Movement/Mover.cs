using RPG.Attributes;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 5f;
        [SerializeField] float maxNavPathLength = 40f;
        NavMeshAgent myNMA;
        Health health;
        
        Ray lastRay;

        private void Awake()
        {
            myNMA = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        private void Update()
        {
            myNMA.enabled = !health.IsDead();
            Debug.DrawRay(lastRay.origin, lastRay.direction * 100);
            UpdateAnimation();
        }
        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath || path.status != NavMeshPathStatus.PathComplete ||
                GetPathLength(path) > maxNavPathLength) return false;
            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }
        

        private void UpdateAnimation()
        {
            Vector3 velocity = myNMA.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("ForwardSpeed", speed);


            // GetComponent<Animator>().SetFloat("ForwardSpeed", Mathf.Abs(myNMA.velocity.z)); like variant
        }



        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            myNMA.speed = maxSpeed;
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            myNMA.speed = Mathf.Clamp01(speedFraction) * maxSpeed;
            myNMA.destination = destination;
            myNMA.isStopped = false;
        }

        public void Cancel()
        {
            myNMA.isStopped = true;          
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector3();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }

}