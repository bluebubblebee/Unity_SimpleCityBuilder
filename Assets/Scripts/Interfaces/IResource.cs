using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public delegate void OnResourceGatheredAction(IResource resource);

    public interface IResource
    {
        event OnResourceGatheredAction OnResourceGathered;

        public ResourceType ResourceType { get; set; }        
        public int TotalHealth { get; set; }
        public int CurrentHealth { get; set; }
        public bool HasBeenGathered { get; set; }
        public void Hit(int damage);
    }
}
