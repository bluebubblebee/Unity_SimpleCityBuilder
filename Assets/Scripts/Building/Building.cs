using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CityBuilder
{
    public class Building : MonoBehaviour, ISelectable, IBuilding
    {
        [SerializeField] private GameObject selectionObject;

        [SerializeField] private GameObject previewObject;

        [SerializeField] private GameObject completedObject;

        public event OnResourceBuiltAction OnBuildCompleted;

        public SelectableType SelectableType { get; set; } = SelectableType.Building;

        public bool IsSelected { get; set; }
        public Vector3 PositionBuild { get; set; }
        public bool HasBeenBuild { get; set; }
        public float TotalBuildProgress { get; set; }
        public float CurrentBuildProgress { get; set; }

        private void Start()
        {
            HasBeenBuild = false;
            previewObject.SetActive(true);
            completedObject.SetActive(false);
        }

        public void ToggleSelection()
        {
            if (!HasBeenBuild) return;

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

        public void StartPlacingBuild()
        {
            HasBeenBuild = false;
            previewObject.SetActive(true);
            completedObject.SetActive(false);
        }

        public void Build(float progress)
        {
            CurrentBuildProgress += progress;

            if (CurrentBuildProgress >= TotalBuildProgress)
            {
                CurrentBuildProgress = TotalBuildProgress;
                HasBeenBuild = true;

                CompleteBuild();
            }
        }

        public void CompleteBuild()
        {
            OnBuildCompleted?.Invoke(this);

            previewObject.SetActive(false);
            completedObject.SetActive(true);
        }
    }
}
