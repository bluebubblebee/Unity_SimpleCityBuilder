using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{    

    public class BuilderController : MonoBehaviour, IController
    {
        public delegate void OnBuilderStateChangeAction(BuilderStateEnum newState);
        public event OnBuilderStateChangeAction OnBuilderStateChange;

        [SerializeField] private BuilderUI builderUI;

        [SerializeField] Material buildingPreviewMat;
        //private Mesh buildingPreviewMesh;
        private GameObject buildingToPlaceObject;       

        private BuildingObjectType buildingToPlace = null;

        public ControllerState State { get; set; }

        public BuilderStateEnum BuilderState { get; set; }

        public void InitializeController()
        {
            State = ControllerState.Initialization;
            BuilderState = BuilderStateEnum.Idle;
            builderUI.Initialize();           
        }

        public IEnumerator SetupController()
        {
            State = ControllerState.Setup;

            for (int i = 0; i< GameManager.instance.GameData.BuildingData.Count; i++ )
            {
                BuildingObjectType building = GameManager.instance.GameData.BuildingData[i];

                builderUI.InitializeResources(building.Type, building.RequiredResources);
            }

           yield break;
        }

        public void StartController()
        {
            builderUI.PotionShopBuilding.BuildingResourceButton.clicked += PotionShopButton_OnClick;
            builderUI.RestTentBuilding.BuildingResourceButton.clicked += RestTentButton_OnClick;

            GameManager.instance.PlayerController.OnResourceUpdated += OnPlayerResourceUpdated;

            State = ControllerState.Running;
        }

        private void OnPlayerResourceUpdated(ResourceType type, int playerResourceAmount)
        {
            builderUI.UpdateRequiredResources(type, playerResourceAmount);
        }        

        private void Update()
        {
            if (State != ControllerState.Running) return;

            if (BuilderState == BuilderStateEnum.PlacingBuilding)
            {                
                RaycastHit hit = MathUtility.CameraRay(GameManager.instance.CameraController.CameraGame);
                Vector3 placeObjectPos = new Vector3(hit.point.x, 0.0f, hit.point.z);
                buildingToPlaceObject.transform.position = placeObjectPos;

                //Debug.Log("<color=cyan>" + "isPlacingBuilding: " + hit.point + "</color>");

                if (Input.GetMouseButtonDown(0)) // Left button, place object
                {
                    buildingToPlaceObject.transform.position = placeObjectPos;

                    GameManager.instance.PlayerController.ReduceResources(buildingToPlace.RequiredResources);

                    BuilderState = BuilderStateEnum.Idle;
                    OnBuilderStateChange?.Invoke(BuilderState);
                }
                else if (Input.GetMouseButtonDown(1)) // Right button, cancle object
                {
                    Destroy(buildingToPlaceObject);
                    BuilderState = BuilderStateEnum.Idle;
                    OnBuilderStateChange?.Invoke(BuilderState);
                }
            }
        }

        public void FinishController()
        {
            State = ControllerState.Completed;

            builderUI.PotionShopBuilding.BuildingResourceButton.clicked -= PotionShopButton_OnClick;
            builderUI.RestTentBuilding.BuildingResourceButton.clicked -= RestTentButton_OnClick;
            GameManager.instance.PlayerController.OnResourceUpdated -= OnPlayerResourceUpdated;
        }        

        private void PotionShopButton_OnClick()
        {
            if (State != ControllerState.Running) return;
            TryStartPlacingBuilding(BuildingType.PotionShop);
        }

        private void RestTentButton_OnClick()
        {
            if (State != ControllerState.Running) return;
            TryStartPlacingBuilding(BuildingType.RestTent);
        }

        private void TryStartPlacingBuilding(BuildingType type)
        {
            if (BuilderState == BuilderStateEnum.PlacingBuilding) return;

            buildingToPlace = null;
            for (int i = 0; i < GameManager.instance.GameData.BuildingData.Count; i++)
            {
                if (GameManager.instance.GameData.BuildingData[i].Type == type)
                {
                    buildingToPlace = GameManager.instance.GameData.BuildingData[i];
                    break;
                }
            }

            if (buildingToPlace == null) return;

            bool hasEnoughResources = GameManager.instance.PlayerController.HasEnoughResources(buildingToPlace.RequiredResources);
            if (!hasEnoughResources)
            {
                GameManager.instance.PlayerController.ShowInGameMessage("Can't build " + type.ToString() + " not enough resources");
                return;
            }

            for (int i = 0; i < GameManager.instance.GameData.BuildingData.Count; i++)
            {
                if (GameManager.instance.GameData.BuildingData[i].Type == type)
                {
                    GameObject prefab = GameManager.instance.GameData.BuildingData[i].Prefab;
                    buildingToPlaceObject = Instantiate(prefab);
                    buildingToPlaceObject.transform.position = Vector3.zero;
                    //buildingPreviewMesh = buildingToPlace.GetComponentInChildren<MeshFilter>().sharedMesh;
                    break;
                }
            }

            BuilderState = BuilderStateEnum.PlacingBuilding;
            OnBuilderStateChange?.Invoke(BuilderState);
        }

    }
}
