using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CityBuilder
{
    public class Building : MonoBehaviour, ISelectable
    {
        [SerializeField] private GameObject selectionObject;
        public SelectableType SelectableType { get; set; } = SelectableType.Building;

        public bool IsSelected { get; set; }

        public void ToggleSelection()
        {
            if (IsSelected)
            {
                IsSelected = false;
                selectionObject.SetActive(false);
            }
            else
            {
                IsSelected = true;
                selectionObject.SetActive(true);
            }
        }
    }
}
