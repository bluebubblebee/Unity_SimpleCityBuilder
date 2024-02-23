using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace CityBuilder
{
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

        private void Start()
        {
            Status = CityCharacterStatus.Idle;
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            float animationSpeed = 0.0f;
            if (navMeshAgent.velocity.magnitude > 0.0f)
            {
                float remainingDistance = (navMeshAgent.remainingDistance - navMeshAgent.stoppingDistance);
                animationSpeed = remainingDistance / navMeshAgent.velocity.magnitude;                
            }
            animator.SetFloat("Speed", Mathf.Clamp(animationSpeed, 0.0f, 1.0f));
           
            if (Status == CityCharacterStatus.GatheringResource)                
            {
                if (currentResource != null)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(currentResourcePosition);
                    const float turnSpeed = 20.0f;
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                }
            }
        }
        public WaitUntil WaitForNavMesh()
        {   
            return new WaitUntil(() => !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);
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

        public void MoveToTarget(Vector3 target)
        {
            if (Status == CityCharacterStatus.GatheringResource)
            {
                StopCurrentTask();
                currentTask = null;
                animator.SetBool("Spellcasting", false);
                currentResource = null;
            }    

            navMeshAgent.destination = target;
        }

        public void GatherResource(IResource resource)
        {            
            currentResource = resource;
            currentResourcePosition = ((ISpawnable)currentResource).CurrentPostion;
            StopCurrentTask();

            resource.OnResourceGathered += Resource_OnResourceGathered;

            currentTask = StartCoroutine(StartGatherResource());
        }

        private void Resource_OnResourceGathered(IResource resource)
        {
            resource.OnResourceGathered -= Resource_OnResourceGathered;

            if (currentResource == resource)
            {
                StopCurrentTask();

                animator.SetBool("Spellcasting", false);
                currentResource = null;

                Status = CityCharacterStatus.Idle;

                UnSelect();
            }
        }

        private IEnumerator StartGatherResource()
        {
            Status = CityCharacterStatus.MovingToTarget;
            
            MoveToTarget(currentResourcePosition);
     
            yield return WaitForNavMesh();

            if ((currentResource != null) && !currentResource.HasBeenGathered)
            {
                Status = CityCharacterStatus.GatheringResource;

                animator.SetBool("Spellcasting", true);

                while ((currentResource != null) && !currentResource.HasBeenGathered)
                {
                    yield return new WaitForSeconds(0.7f);
                    currentResource.Hit(attackStrength);
                }

                if ((currentResource != null) && currentResource.HasBeenGathered)
                {
                    Status = CityCharacterStatus.Idle;
                    animator.SetBool("Spellcasting", false);

                    currentResource.OnResourceGathered -= Resource_OnResourceGathered;

                    currentResource = null;
                    currentTask = null;

                    UnSelect();
                }
            }
        }

        private void StopCurrentTask()
        {
            if (currentTask != null)
            {   
                StopCoroutine(currentTask);
            }
        }

    }
}