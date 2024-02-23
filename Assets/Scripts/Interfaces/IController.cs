using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{   
    public enum ControllerState
    {
        Initialization,
        Setup,
        Running,
        Completed
    }

    public interface IController
    {
        public ControllerState State { get; set; }
        public void InitializeController();
        public IEnumerator SetupController();
        public void StartController();
        public void FinishController();
    }
}
