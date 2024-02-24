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
        [SerializeField] private int buildSpeed;

        private IResource currentResource;
        private IBuilding currentBuilding;

        private Vector3 nextPosition;
        private Coroutine currentTask;

        private Queue<CityCharacterTask> taskQueue;
        private CityCharacterTask currentCityTask;
        private float totalDistanceToTarget;
        private float stoppingDistance = 8.0f;

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
            currentTask = null;
            taskQueue = new Queue<CityCharacterTask>();
            currentCityTask = null;

            UnSelect();
            Status = CityCharacterStatus.Free;
            animator.SetBool("Spellcasting", false);            
        }

        private bool CanChangeTask()
        {
            if (Status == CityCharacterStatus.Busy)
            { 
                if ((currentCityTask != null) && (!currentCityTask.Completed))
                {
                    if ((currentCityTask.TaskType == BuilderTask.GatheringResource) || (currentCityTask.TaskType == BuilderTask.Build))
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

            if (!IsSelected) return;

            StopCurrentTask();

            taskQueue = new Queue<CityCharacterTask>();

            CityCharacterTask task0 = new CityCharacterTask();
            task0.TaskType = BuilderTask.MovingToTarget;
            task0.Completed = false;
            
            taskQueue.Enqueue(task0);

            Status = CityCharacterStatus.Busy;
            navMeshAgent.destination = target;

            stoppingDistance = 4.0f;
            navMeshAgent.stoppingDistance = stoppingDistance;            

            totalDistanceToTarget = Vector3.Distance(transform.position, navMeshAgent.destination);

            Status = CityCharacterStatus.Busy;

            Select();
            currentTask = StartCoroutine(HandleQueueTasks());
        }

        public void GatherResource(IResource resource)
        {
            if (!IsSelected) return;

            StopCurrentTask();

            currentResource = resource;
            nextPosition = ((ISpawnable)currentResource).CurrentPostion;

            CityCharacterTask task0 = new CityCharacterTask();
            task0.TaskType = BuilderTask.MovingToTarget;
            task0.Completed = false;

            CityCharacterTask task1 = new CityCharacterTask();
            task1.TaskType = BuilderTask.GatheringResource;
            task1.Completed = false;

            taskQueue = new Queue<CityCharacterTask>();

            taskQueue.Enqueue(task0);
            taskQueue.Enqueue(task1);

            totalDistanceToTarget = Vector3.Distance(transform.position, nextPosition);            

            Status = CityCharacterStatus.Busy;

            navMeshAgent.destination = nextPosition;
            stoppingDistance = 8.0f;
            navMeshAgent.stoppingDistance = stoppingDistance;

            currentTask = StartCoroutine(HandleQueueTasks());        
        }

        public void SendToBuild(IBuilding building)
        {
            if (Status == CityCharacterStatus.Busy) return;

            StopCurrentTask();

            currentBuilding = building;
            nextPosition = currentBuilding.PositionBuild;

            CityCharacterTask task0 = new CityCharacterTask();
            task0.TaskType = BuilderTask.MovingToTarget;
            task0.Completed = false;

            CityCharacterTask task1 = new CityCharacterTask();
            task1.TaskType = BuilderTask.Build;
            task1.Completed = false;

            taskQueue = new Queue<CityCharacterTask>();

            taskQueue.Enqueue(task0);
            taskQueue.Enqueue(task1);

            totalDistanceToTarget = Vector3.Distance(transform.position, nextPosition);

            Status = CityCharacterStatus.Busy;

            navMeshAgent.destination = nextPosition;
            stoppingDistance = 21.0f;
            navMeshAgent.stoppingDistance = stoppingDistance;

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
                    case BuilderTask.Build:
                        yield return WaitToBuild();
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

            while (totalDistanceToTarget > stoppingDistance)
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
                transform.LookAt(nextPosition, Vector3.up);

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

        private IEnumerator WaitToBuild()
        {
            if ((currentBuilding != null) && !currentBuilding.HasBeenBuild)
            {
                transform.LookAt(nextPosition, Vector3.up);

                animator.SetBool("Spellcasting", true);

                while ((currentBuilding != null) && !currentBuilding.HasBeenBuild)
                {
                    yield return new WaitForSeconds(0.7f);
                    currentBuilding.Build(buildSpeed);
                }
            }

            animator.SetBool("Spellcasting", false);
            currentBuilding = null;
            UnSelect();
        }

        public void ToggleSelection()
        {
            if (IsSelected)
            {
                UnSelect();
            }
            else
            {
                Select();
            }
        }

        private void Select()
        {
            IsSelected = true;
            selection.SetActive(true);
        }

        private void UnSelect()
        {
            IsSelected = false;
            selection.SetActive(false);
        }
    }
}