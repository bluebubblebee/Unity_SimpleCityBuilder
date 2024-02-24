using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public interface ICityCharacter
    {
        public CityCharacterStatus Status { get; set; }
        public void MoveToTarget(Vector3 target);
        public void GatherResource(IResource resource);
        public void SendToBuild(IBuilding building);
    }
}