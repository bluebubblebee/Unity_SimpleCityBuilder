using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CityBuilder
{
    public class MapController : MonoBehaviour, IController
    {
        public ControllerState State { get; set; }

        [SerializeField] private int initialNumberOfBuilders = 3;
        [SerializeField] private List<Transform> builderSpawnPositions;

        public List<CityCharacter> CityCharacters { get; private set; }

        [SerializeField] private AssetReference builderAssetReference;
        private GameObject builderGameObjectRef;

        public void InitializeController()
        {
            State = ControllerState.Initialization;
            CityCharacters = new List<CityCharacter>();
        }

        public IEnumerator SetupController()
        {
            State = ControllerState.Setup;

            yield return InstanceInitialBuilders();            
        }

        private IEnumerator InstanceInitialBuilders()
        {
            // Load a builder
            if (!builderAssetReference.RuntimeKeyIsValid())
            {
                Debug.Log("<color=cyan>" + " Invalid builder key addressable " + "</color>");
                yield break;
            }

            AsyncOperationHandle<GameObject> builderLoadOpHandle = builderAssetReference.LoadAssetAsync<GameObject>();
            while (!builderLoadOpHandle.IsDone)
            {
                yield return null;
            }

            Debug.Log("<color=cyan>" + " Loaded builder addressable - Status: " + builderLoadOpHandle.Status + "</color>");

            if (builderLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
            {
                builderGameObjectRef = builderLoadOpHandle.Result;

                if (builderGameObjectRef == null) yield break;

                if (builderSpawnPositions.Count < initialNumberOfBuilders) yield break;

                for (int i = 0; i < initialNumberOfBuilders; i++)
                {
                    GameObject charInstance = Instantiate(builderGameObjectRef, transform);

                    charInstance.transform.SetPositionAndRotation(builderSpawnPositions[i].position, Quaternion.Euler(0.0f, 180.0f, 0.0f));

                    CityCharacter character = charInstance.GetComponent<CityCharacter>();
                    if (character)
                    {
                        CityCharacters.Add(character);
                    }
                }
            }

            Debug.Log("<color=cyan>" + " Builders instanced: " + CityCharacters.Count + "</color>");
        }

        public void StartController()
        {
            GameManager.instance.CameraController.OnHitObject += OnCameraControllerHitObject;
            GameManager.instance.CameraController.OnHitPoint += CameraControllerHitPoint;

            State = ControllerState.Running;
        }

        public void FinishController()
        {
            GameManager.instance.CameraController.OnHitObject -= OnCameraControllerHitObject;
            GameManager.instance.CameraController.OnHitPoint -= CameraControllerHitPoint;

            State = ControllerState.Completed;
        }


        private void OnCameraControllerHitObject(ISelectable hitObject)
        {
            switch (hitObject.SelectableType)
            {
                case SelectableType.Resource:

                    for (int i = 0; i < CityCharacters.Count; i++)
                    {
                        CityCharacter cityChar = CityCharacters[i];
                        if (/*(cityChar.Status == CityCharacterStatus.Free) && */(cityChar.IsSelected))
                        {
                            cityChar.GatherResource((IResource)hitObject);
                        }
                    }

                    break;
            }
        }

        private void CameraControllerHitPoint(Vector3 point)
        {
            for (int i = 0; i < CityCharacters.Count; i++)
            {
                CityCharacter cityChar = CityCharacters[i];

                if (/*(cityChar.Status == CityCharacterStatus.Free) &&*/ (cityChar.IsSelected))
                {
                    cityChar.MoveToTarget(point);
                }
            }

        }

    }
}
