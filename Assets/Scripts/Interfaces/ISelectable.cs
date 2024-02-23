using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CityBuilder
{
    public interface ISelectable
    {
        public SelectableType SelectableType { get; set; }

        public bool IsSelected { get; set; }

        public void ToggleSelection();
    }
}
