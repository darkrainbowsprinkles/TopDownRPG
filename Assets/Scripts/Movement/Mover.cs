using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, ICancelable, ISaveable
    {
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;
        NavMeshAgent agent;
        Animator animator;
        ActionScheduler actionScheduler;

        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new();

            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

            if(!hasPath) 
            {
                return false;
            }

            if(path.status != NavMeshPathStatus.PathComplete) 
            {
                return false;
            }

            if(GetPathLength(path) > maxNavPathLength) 
            {
                return false;
            }

            return true;
        }

        public void Teleport(Vector3 destination)
        {
            agent.Warp(destination);
        }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
        }
        
        void Update()
        {
            animator.SetFloat("forwardSpeed", GetLocalSpeed());
        }

        void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.enabled = true;
            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
        }

        float GetPathLength(NavMeshPath path)
        {
            float total = 0f;
            float length = path.corners.Length;

            if(length < 2f) 
            {
                return total;
            }

            for (int i = 0; i < length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        float GetLocalSpeed()
        {
            Vector3 globalVelocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(globalVelocity);
            return localVelocity.z;
        }

        public void Cancel()
        {
            if(agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
            }
        }

        object ISaveable.CaptureState()
        {
            MoverSaveData data = new();

            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);

            return data;
        }

        void ISaveable.RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;

            agent.enabled = false;

            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();

            agent.enabled = true;

            actionScheduler.CancelCurrentAction();
        }
    }
}

