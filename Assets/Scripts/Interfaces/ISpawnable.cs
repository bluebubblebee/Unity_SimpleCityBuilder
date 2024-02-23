using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public delegate void SpawnAction(ISpawnable spawnObject);
    public interface ISpawnable
    {
        event SpawnAction OnActivateSpawn;
        event SpawnAction OnDeactivateSpawn;
        public bool IsActive { get; set; }
        public Vector3 InitialPosition { get; set; }
        public Quaternion InitialRotation { get; set; }
        public Vector3 CurrentPostion { get; set; }
        public Quaternion CurrentRotation { get; set; }

        public void ActiveSpawn(Vector3 SpawnPosition, Quaternion SpawnRotation);

        public GameObject GetSpawnObject { get; set; }
        public void DisableSpawn();
    }
}
