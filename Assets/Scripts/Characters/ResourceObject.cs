using UnityEngine;
using DG.Tweening;

namespace CityBuilder
{
    public class ResourceObject : MonoBehaviour, IResource, ISpawnable, ISelectable
    {
        //[SerializeField] private ResourceType type;
        [SerializeField] private GameObject selectionObject;

        public event SpawnAction OnActivateSpawn;
        public event SpawnAction OnDeactivateSpawn;

        public event OnResourceGatheredAction OnResourceGathered;
        public ResourceType ResourceType { get; set; }

        public int Amount { get; set; }
        public int TotalHealth { get; set; }
        public int CurrentHealth { get; set; }
        public bool HasBeenGathered { get; set; }
        public bool IsActive { get; set; }
        public Vector3 InitialPosition { get; set; }
        public Quaternion InitialRotation { get; set; }
        public Vector3 CurrentPostion { get; set; }
        public Quaternion CurrentRotation { get; set; }
        public GameObject GetSpawnObject { get { return gameObject; } set { } }

        public SelectableType SelectableType { get; set; } = SelectableType.Resource;
        public bool IsSelected { get ; set ; }
        

        void Start()
        {
            gameObject.SetActive(false);
            IsActive = false;
            HasBeenGathered = false;
        }

        public void ActiveSpawn(Vector3 SpawnPosition, Quaternion SpawnRotation)
        {
            gameObject.SetActive(true);

            HasBeenGathered = false;

            CurrentPostion = SpawnPosition;
            CurrentRotation = SpawnRotation;

            transform.position = SpawnPosition;
            transform.rotation = SpawnRotation;

            IsActive = true;
            OnActivateSpawn?.Invoke(this);
        }

        public void Hit(int damage)
        {
            CurrentHealth -= damage;
            transform.DOComplete();
            transform.DOShakeScale(.5f, .2f, 10, 90, true);

            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
                HasBeenGathered = true;

                OnResourceGathered?.Invoke(this);

                GameManager.instance.PlayerController.AddResource(ResourceType, Amount);

                DisableSpawn();
            }
        }

        public void DisableSpawn()
        {
            IsActive = false;

            transform.position = InitialPosition;
            transform.rotation = InitialRotation;

            CurrentPostion = InitialPosition;
            CurrentRotation = InitialRotation;

            OnDeactivateSpawn?.Invoke(this);

            gameObject.SetActive(false);
        }

        public void ToggleSelection()
        {
            if (IsSelected)
            {
                IsSelected = false;
                selectionObject.SetActive(false);
            }
            else
            {
                IsSelected = true;
                selectionObject.SetActive(true);
            }
        
        }
    }
}
