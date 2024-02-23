using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class SpawnFactory : MonoBehaviour
    {
        private Dictionary<string, List<ISpawnable>> poolDictionary;

        public void InitializenFactory()
        {
            poolDictionary = new Dictionary<string, List<ISpawnable>>();
        }

        public List<ISpawnable> CreateListSpawns(string key, int numberSpawns, GameObject objectToSpawn, float offsetY)
        {
            List<ISpawnable> list = new List<ISpawnable>();

            Vector3 spawnPosition = new Vector3(0, offsetY, 0.0f);
            float spawnXOffset = 3;

            for (int i = 0; i < numberSpawns; i++)
            {
                GameObject spawnedObject = Instantiate(objectToSpawn, transform);

                ISpawnable spawn = spawnedObject.GetComponent<ISpawnable>();
                if (spawn != null)
                {
                    spawnPosition.x = (i * spawnXOffset);
                    spawn.InitialPosition = spawnPosition;
                    spawn.InitialRotation = Quaternion.identity;
                    spawn.CurrentPostion = spawnPosition;
                    spawn.CurrentRotation = Quaternion.identity;

                    //spawn.Create(spawnPosition, Quaternion.identity);
                    list.Add(spawn);
                }
            }

            if (poolDictionary.ContainsKey(key))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    poolDictionary[key].Add(list[i]);
                }
            }
            else
            {
                poolDictionary.Add(key, list);
            }
            return list;
        }

        public ISpawnable GetSpawn(string key)
        {
            if (poolDictionary.ContainsKey(key))
            {
                for (int i = 0; i < poolDictionary[key].Count; i++)
                {
                    if (!poolDictionary[key][i].IsActive)
                    {
                        return poolDictionary[key][i];
                    }
                }
            }

            return null;
        }

        public void ClearFactory()
        {
            foreach (KeyValuePair<string, List<ISpawnable>> entry in poolDictionary)
            {
                for (int i = (entry.Value.Count - 1); i >= 0; i--)
                {
                    Destroy(entry.Value[i].GetSpawnObject);
                }
            }
        }

    }
}
