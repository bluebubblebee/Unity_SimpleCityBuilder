using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public enum GameState
    {
        Initialization,
        SetupGame,
        StartGame,
        InGame,
        GameOver
    }

    public enum SelectableType
    {
        CityCharacter,
        Building,
        Resource,
    }

    public enum BuildingType
    {
        PotionShop,
        RestTent,
    };

    public enum BuilderStateEnum
    {
        Idle,
        PlacingBuilding,
    }; 

    public enum CityCharacterStatus
    {
        Free,
        Busy,
    };

    public enum BuilderTask
    {
        None,
        MovingToTarget,
        GatheringResource,
        Build,
    };



    [System.Serializable]
    public class BuildingObjectType
    {
        public BuildingType Type;

        public float TotalTimeToBuild;

        public GameObject Prefab;

        [Header("Resource Data")]
        public List<RequiredResource> RequiredResources;
    }
    
}
