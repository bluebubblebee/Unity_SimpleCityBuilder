using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CityBuilder
{
    public class CameraController : MonoBehaviour, IController
    {
        public delegate void OnHitObjectAction(ISelectable hitObject);
        public event OnHitObjectAction OnHitObject;

        public delegate void OnHitPointAction(Vector3 point);
        public event OnHitPointAction OnHitPoint;
        public ControllerState State { get; set; }

        [SerializeField] private Camera cameraGame;
        
        [SerializeField] private float cameraSpeed;

        [SerializeField] private Vector2 zoomLimits;

        private Transform mainCamera;
        [SerializeField] private Transform zoomObject;

        public Camera CameraGame { get { return cameraGame; } set { cameraGame = value; } }
        private Vector3 moveInput;

        public void InitializeController()
        {
            State = ControllerState.Initialization;
            mainCamera = cameraGame.transform;
            transform.LookAt(mainCamera);
        }

        public IEnumerator SetupController()
        {
            State = ControllerState.Setup;
            yield break;
        }

        public void StartController()
        {
            State = ControllerState.Running;
        }

        public void FinishController()
        {
            State = ControllerState.Completed;
        }

        void Update()
        {
            if (State != ControllerState.Running) return;

            UpdateCameraMovement();

            CheckRayForObjectHits();
        }

        private void UpdateCameraMovement()
        {
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");
            moveInput.z = 0.0f;

            Vector2 mousePos = Input.mousePosition;

            // Restrict cameraMovenet
            float maxPercent = 0.95f;
            float minPercet = 0.05f;
            if (mousePos.x > Screen.width * maxPercent && mousePos.x < Screen.width)
            {
                moveInput.x = 1;
            }
            else if (mousePos.x < Screen.width * minPercet && mousePos.x > 0)
            {
                moveInput.x = -1;
            }

            if (mousePos.y > Screen.height * maxPercent && mousePos.y < Screen.height)
            {
                moveInput.z = 1;
            }
            else if (mousePos.y < Screen.height * minPercet && mousePos.y > 0)
            {
                moveInput.z = -1;
            }

            Vector3 directionMove = mainCamera.TransformDirection(moveInput);
            directionMove.y = 0;
            mainCamera.position += directionMove.normalized * Time.deltaTime * cameraSpeed;

        }               

        private void CheckRayForObjectHits()
        {
            if (Input.GetMouseButtonDown(0))
            {
                float distanceToCheck = 1000;

                RaycastHit hitInfo;
                if (Physics.Raycast(cameraGame.ScreenPointToRay(Input.mousePosition), out hitInfo, distanceToCheck))
                {

                    ISelectable selectedObject = hitInfo.collider.GetComponent<ISelectable>();
                    if (selectedObject != null)
                    {
                        selectedObject.ToggleSelection();

                        OnHitObject?.Invoke(selectedObject);
                    }
                    else
                    {
                        Vector3 hitPosition = new Vector3(hitInfo.point.x, 0.0f, hitInfo.point.z);

                        OnHitPoint?.Invoke(hitPosition);
                    }
                }                   

            }
        }        
    }
}
