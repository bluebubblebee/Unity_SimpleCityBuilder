using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CityBuilder
{
    public delegate void OnResourceBuiltAction(IBuilding building);
    public interface IBuilding 
    {
        event OnResourceBuiltAction OnBuildCompleted;

        public Vector3 PositionBuild { get; set; }

        public bool HasBeenBuild { get; set; }
        public float TotalBuildProgress { get; set; }
        public float CurrentBuildProgress { get; set; }

        public void StartPlacingBuild();

        public void CompleteBuild();

        public void Build(float progress);
    }
}
