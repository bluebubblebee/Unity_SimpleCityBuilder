using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{    

    [CreateAssetMenu(fileName = "GameData", menuName = "CityBuilder/GameData", order = 1)]
    public class GameData : ScriptableObject
    {
        public List<ResourceObjectType> GameResources; 

        public List<BuildingObjectType> BuildingData;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }

        private GameState gameState = GameState.Initialization;

        [SerializeField] private GameData gameData; 

        public GameData GameData { get; private set; }
        public ResourcesController ResourcesController { get; private set; }
        public BuilderController BuilderController { get; private set; }

        public CameraController CameraController { get; private set; }
        public PlayerController PlayerController { get; private set; }


        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            GameData = gameData;
            gameState = GameState.Initialization;

            ResourcesController = GetComponentInChildren<ResourcesController>();
            if (ResourcesController)
            {
                ResourcesController.InitializeController();
            }

            BuilderController = GetComponent<BuilderController>();
            if (BuilderController)
            {
                BuilderController.InitializeController();
            }

            PlayerController = GetComponent<PlayerController>();
            if (PlayerController)
            {
                PlayerController.InitializeController();
            }

            CameraController = GetComponent<CameraController>();
            if (CameraController)
            {
                CameraController.InitializeController();
            }

            StartCoroutine(SetupGame());            
        }

        private IEnumerator SetupGame()
        {
            gameState = GameState.SetupGame;

            yield return ResourcesController.SetupController();
            yield return PlayerController.SetupController();
            yield return BuilderController.SetupController();
            yield return CameraController.SetupController();

            StartGame();
        }

        private void StartGame()
        {
            ResourcesController.StartController();
            PlayerController.StartController();
            BuilderController.StartController();
            CameraController.StartController();

            gameState = GameState.StartGame;
        }
    }
}
