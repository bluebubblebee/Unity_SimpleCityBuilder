using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class PlayerController : MonoBehaviour, IController
    {
        public delegate void OnResourceUpdatedDelegate(ResourceType type, int amount);
        public OnResourceUpdatedDelegate OnResourceUpdated;

        [SerializeField] private PlayerUI playerUI;

        private Dictionary<ResourceType, int> playerResources;

        [SerializeField] private List<CityCharacter> cityCharacters;

        public ControllerState State { get; set; }

        public void InitializeController()
        {
            State = ControllerState.Initialization;

            playerResources = new Dictionary<ResourceType, int>();
            playerResources.Add(ResourceType.Wood, 0);
            playerResources.Add(ResourceType.Stone, 0);
            playerResources.Add(ResourceType.Money, 0);
            playerResources.Add(ResourceType.Mana, 100);
            playerResources.Add(ResourceType.Food, 100);

            playerUI.Initialize();

            UpdateUI();
        }

        public int GetResource(ResourceType type)
        {
            if (playerResources.ContainsKey(type))
            {
                return playerResources[type];
            }

            return 0;
        }

        public IEnumerator SetupController()
        {
            State = ControllerState.Setup;

            yield break;
        }
        public void StartController()
        {
            State = ControllerState.Running;            

            GameManager.instance.CameraController.OnHitObject += OnCameraControllerHitObject;
            GameManager.instance.CameraController.OnHitPoint += CameraControllerHitPoint;
        }
               

        private void OnCameraControllerHitObject(ISelectable hitObject)
        {
            switch (hitObject.SelectableType)
            {
                case SelectableType.Resource:

                    for (int i = 0; i < cityCharacters.Count; i++)
                    {
                        if ((cityCharacters[i].Status == CityCharacterStatus.Idle) && (cityCharacters[i].IsSelected))
                        {
                            cityCharacters[i].GatherResource((IResource)hitObject);
                        }
                    }

                    break;

            }
        }

        private void CameraControllerHitPoint(Vector3 point)
        {
            for (int i=0; i< cityCharacters.Count; i++)
            {
                if ((cityCharacters[i].Status == CityCharacterStatus.Idle) && (cityCharacters[i].IsSelected))
                {
                    cityCharacters[i].MoveToTarget(point);
                }
            }
        }        


        public void AddResource(ResourceType type, int amount)
        {
            if (playerResources.ContainsKey(type))
            {
                playerResources[type] += amount;
                playerResources[type] = Mathf.Clamp(playerResources[type], 0, 1000000);
                OnResourceUpdated?.Invoke(type, playerResources[type]);
            }

            UpdateUI();
        }
        public void SetResource(ResourceType type, int amount)
        {
            if(playerResources.ContainsKey(type))
            {
                playerResources[type] = amount;
                playerResources[type] = Mathf.Clamp(playerResources[type], 0, 1000000);
                OnResourceUpdated?.Invoke(type, playerResources[type]);
            }

            UpdateUI();
        }

        public bool HasEnoughResources(List<RequiredResource> resources)
        {
            for (int i=0;i<resources.Count; i++)
            {
                if (playerResources.ContainsKey(resources[i].Type))
                {
                    if (playerResources[resources[i].Type] < resources[i].Amount)
                    {
                        return false;
                    }
                }                   
            }

            return true;
        }

        public void ReduceResources(List<RequiredResource> resources)
        {
            for (int i = 0; i < resources.Count; i++)
            {
                if (playerResources.ContainsKey(resources[i].Type))
                {
                    playerResources[resources[i].Type] -= resources[i].Amount;
                    playerResources[resources[i].Type] = Mathf.Clamp(playerResources[resources[i].Type], 0, 1000);
                    OnResourceUpdated?.Invoke(resources[i].Type, playerResources[resources[i].Type]);
                }
            }

            UpdateUI();
        }

        public void ShowInGameMessage(string text)
        {
            if (!playerUI) return;

            playerUI.ShowInGameMessage(text);
        }

        private void UpdateUI()
        {
            if (!playerUI) return;

            playerUI.WoodLabel.text = "Wood: " + playerResources[ResourceType.Wood];           
            playerUI.StoneLabel.text = "Stone: " + playerResources[ResourceType.Stone];
            playerUI.ManaLabel.text = "Mana: " + playerResources[ResourceType.Mana];
        }


        public void FinishController()
        {
            GameManager.instance.CameraController.OnHitObject -= OnCameraControllerHitObject;
            GameManager.instance.CameraController.OnHitPoint -= CameraControllerHitPoint;

            State = ControllerState.Completed;
        }

    }
}
