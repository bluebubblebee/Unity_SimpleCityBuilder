using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace CityBuilder
{
    public class CityCharacterTask
    {
        public BuilderTask TaskType;
        public bool Completed;

    }


    public class CityCharacter : MonoBehaviour, ICityCharacter, ISelectable
    {
        public CityCharacterStatus Status { get; set; }
        public SelectableType SelectableType { get; set; } = SelectableType.CityCharacter;
        public bool IsSelected { get; set; } = false;

        private NavMeshAgent navMeshAgent;
        [SerializeField] private GameObject selection;
        [SerializeField] private Animator animator;
        [SerializeField] private int attackStrength;

        private IResource currentResource;
        private Vector3 currentResourcePosition;
        private Coroutine currentTask;

        private Queue<CityCharacterTask> taskQueue;
        private CityCharacterTask currentCityTask;
        private float totalDistanceToTarget;

        private void Start()
        {
            Status = CityCharacterStatus.Free;
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void StopCurrentTask()
        {
            if (currentTask != null)
            {
                StopCoroutine(currentTask);                
            }

            taskQueue = new Queue<CityCharacterTask>();
            Status = CityCharacterStatus.Free;
            animator.SetBool("Spellcasting", false);
        }

        private bool CanChangeTask()
        {
            if (Status == CityCharacterStatus.Busy)
            { 
                if ((currentCityTask != null) && (!currentCityTask.Completed))
                {
                    if (currentCityTask.TaskType == BuilderTask.GatheringResource)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void MoveToTarget(Vector3 target)
        {
            if (!CanChangeTask()) return;

            StopCurrentTask();

            taskQueue = new Queue<CityCharacterTask>();

            CityCharacterTask task0 = new CityCharacterTask();
            task0.TaskType = BuilderTask.MovingToTarget;
            task0.Completed = false;
            
            taskQueue.Enqueue(task0);

            Status = CityCharacterStatus.Busy;
            navMeshAgent.destination = target;
            navMeshAgent.stoppingDistance = 4;

            totalDistanceToTarget = Vector3.Distance(transform.position, navMeshAgent.destination);

            Status = CityCharacterStatus.Busy;
            currentTask = StartCoroutine(HandleQueueTasks());
        }


        public void GatherResource(IResource resource)
        {
            StopCurrentTask();

            currentResource = resource;
            currentResourcePosition = ((ISpawnable)currentResource).CurrentPostion;

            CityCharacterTask task0 = new CityCharacterTask();
            task0.TaskType = BuilderTask.MovingToTarget;
            task0.Completed = false;

            CityCharacterTask task1 = new CityCharacterTask();
            task1.TaskType = BuilderTask.GatheringResource;
            task1.Completed = false;

            taskQueue = new Queue<CityCharacterTask>();

            taskQueue.Enqueue(task0);
            taskQueue.Enqueue(task1);
            
            navMeshAgent.destination = currentResourcePosition;
            navMeshAgent.stoppingDistance = 8;

            totalDistanceToTarget = Vector3.Distance(transform.position, navMeshAgent.destination);

            Status = CityCharacterStatus.Busy;
            currentTask = StartCoroutine(HandleQueueTasks());

        }

        private IEnumerator HandleQueueTasks()
        {
            Debug.Log(" Total tasks " + taskQueue.Count);

            while (taskQueue.Count > 0)
            {
                currentCityTask = taskQueue.Dequeue();

                Debug.Log(" Waiting New completed " + currentCityTask.TaskType + " - taskQueue.Count: " + taskQueue.Count);
                switch (currentCityTask.TaskType)
                {
                    case BuilderTask.MovingToTarget:
                        yield return WaitToMoveToTargetTask();
                        break;
                    case BuilderTask.GatheringResource:
                        yield return WaitToGatherResource();
                        break;

                }

                currentCityTask.Completed = true;
                Debug.Log(" Current Task completed " + currentCityTask.TaskType + " - taskQueue.Count: " + taskQueue.Count);
            }

            Status = CityCharacterStatus.Free;
        }

        private IEnumerator WaitToMoveToTargetTask()
        {
            navMeshAgent.isStopped = false;

            while (totalDistanceToTarget > 8.0f)
            {
                totalDistanceToTarget = Vector3.Distance(transform.position, navMeshAgent.destination);

                float remainingDistance = (navMeshAgent.remainingDistance - navMeshAgent.stoppingDistance);
                float animationSpeed = remainingDistance / navMeshAgent.velocity.magnitude;

                animator.SetFloat("Speed", Mathf.Clamp(animationSpeed, 0.0f, 1.0f));
                yield return null;
            }

            animator.SetFloat("Speed", 0.0f);

            navMeshAgent.isStopped = true;
        }


        private IEnumerator WaitToGatherResource()
        {
            if ((currentResource != null) && !currentResource.HasBeenGathered)
            {
                transform.LookAt(currentResourcePosition, Vector3.up);

                animator.SetBool("Spellcasting", true);

                while ((currentResource != null) && !currentResource.HasBeenGathered)
                {
                    yield return new WaitForSeconds(0.7f);
                    currentResource.Hit(attackStrength);
                }
            }

            animator.SetBool("Spellcasting", false);
            currentResource = null;
            UnSelect();

            yield break;
        }              


        public void ToggleSelection()
        {
            if (IsSelected)
            {
                UnSelect();
            }
            else
            {
                IsSelected = true;
                selection.SetActive(true);
            }
        }

        private void UnSelect()
        {
            IsSelected = false;
            selection.SetActive(false);
        }

    }
}