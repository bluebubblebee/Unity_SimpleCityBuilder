using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{    
    public enum ResourceType
    {
        Wood,
        Stone,
        Food
    };

    [System.Serializable]
    public class RequiredResource
    {
        public ResourceType Type;
        public int Amount;
    }

    [System.Serializable]
    public class ResourceObjectType
    {
        [Header("Resource Data")]
        public ResourceType Type;

        public int AmountResource;

        public int TotalHealth;

        [Header("Pool Data")]
        public int SizePool;

        public GameObject prefab;        
    }  


    public class ResourcesController : MonoBehaviour, IController
    {
        [SerializeField] private float xMinSpawnLoc = -108.0f;
        [SerializeField] private float xMaxSpawnLoc = 0.0f;
        [SerializeField] private float zMinSpawnLoc = 80.0f;
        [SerializeField] private float zMaxSpawnLoc = 80.0f;

        public ControllerState State { get; set; }

        private SpawnFactory spawnFactory;
        private int currentActiveSpawns;

        public void InitializeController()
        {
            State = ControllerState.Initialization;

            spawnFactory = GetComponent<SpawnFactory>();
            spawnFactory.InitializenFactory();            
        }

        public IEnumerator SetupController()
        {
            State = ControllerState.Setup;
            CreateInstances();

            yield break;
        }

        private void CreateInstances()
        {
            for (int j = 0; j < GameManager.instance.GameData.GameResources.Count; j++)
            {
                ResourceObjectType resourceObject = GameManager.instance.GameData.GameResources[j];

                List<ISpawnable> spawnList = spawnFactory.CreateListSpawns(resourceObject.Type.ToString(), resourceObject.SizePool, resourceObject.prefab, -20.0f);

                for (int z = 0; z < spawnList.Count; z++)
                {

                    ResourceObject resourceSpawned = spawnList[z].GetSpawnObject.GetComponent<ResourceObject>();
                    if (resourceSpawned != null)
                    {
                        resourceSpawned.ResourceType = resourceObject.Type;
                        resourceSpawned.TotalHealth = resourceObject.TotalHealth;
                        resourceSpawned.CurrentHealth = resourceObject.TotalHealth;
                        resourceSpawned.HasBeenGathered = false;

                        resourceSpawned.Amount = resourceObject.AmountResource;
                    }
                }
            }            
        }        

        public void StartController()
        {
            State = ControllerState.Running;
            
            // 5 Rocks, 5 Trees
            for (int i = 0; i < 5; i++)
            {
                SpawnByType(ResourceType.Wood);

            }

            for (int i = 0; i < 5; i++)
            {
                SpawnByType(ResourceType.Stone);
            }
        }

        private void SpawnByType(ResourceType type)
        {
            ISpawnable spawn = spawnFactory.GetSpawn(type.ToString());

            if (spawn != null)
            {
                spawn.OnDeactivateSpawn += OnDeactivateSpawn;

                Vector3 spawnPos = Vector3.zero;
                spawnPos.x = UnityEngine.Random.Range(xMinSpawnLoc, xMaxSpawnLoc);
                spawnPos.z = UnityEngine.Random.Range(zMinSpawnLoc, zMaxSpawnLoc);


                float rotationVariance = 90;
                Vector3 rot = Vector3.zero;
                rot.y = UnityEngine.Random.Range(-rotationVariance, rotationVariance);
                Quaternion randomRot = Quaternion.Euler(rot);

                spawn.ActiveSpawn(spawnPos, randomRot);

            }
        }

        private void OnDeactivateSpawn(ISpawnable spawnObject)
        {
            spawnObject.OnDeactivateSpawn -= OnDeactivateSpawn;

            currentActiveSpawns--;

            if (currentActiveSpawns < 0)
            {
                currentActiveSpawns = 0;
            }
        }

        public void FinishController()
        {
            spawnFactory.ClearFactory();

            State = ControllerState.Completed;
        }
    }
}
