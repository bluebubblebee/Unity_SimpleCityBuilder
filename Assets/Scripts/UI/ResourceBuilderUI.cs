using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CityBuilder
{

    [System.Serializable]
    public class RequiredResourceElementUI
    {
        public string Title;
        public Label ValueLabel;
        public string Value;
        public int TotalRequired;
    }

    public class ResourceBuilderUI
    {
        private const string enoughResourceTextColor = "<color=#3adb2a>";
        private const string missingResourceTextColor = "<color=#e9511e>";

        private TemplateContainer resourceBuildingTemplate;

        private BuildingType buildingType;

        public Button BuildingResourceButton
        {
            get; private set;
        }

        private Label buildingResourceTitle;

        public List<RequiredResource> RequiredResources { get; private set; }

        private Dictionary<ResourceType, RequiredResourceElementUI> buildingRequiredResources;

        public void SetResource(BuildingType type, TemplateContainer resource)
        {
            buildingType = type;
            resourceBuildingTemplate = resource; 
            if (resourceBuildingTemplate != null)
            {
                BuildingResourceButton = resourceBuildingTemplate.Query<Button>("BuildingButton");

                buildingResourceTitle = resourceBuildingTemplate.Query<Label>("BuildingTypeLabel");
            }

            if (buildingResourceTitle != null)
            {
                switch(type)
                {
                    case BuildingType.PotionShop:
                        buildingResourceTitle.text = "Potion Shop";
                        break;
                    case BuildingType.RestTent:
                        buildingResourceTitle.text = "Rest Tent";
                        break;
                }
            }

            if (BuildingResourceButton != null)
            {
                BuildingResourceButton.RegisterCallback<NavigationSubmitEvent>(BuildingResourceButton_NavigationSubmitEvent, TrickleDown.TrickleDown);
            }
        }

        private void BuildingResourceButton_NavigationSubmitEvent(NavigationSubmitEvent submitEvent)
        { }

        public void DesRegisterEvents()
        {
            if (BuildingResourceButton != null)
            {
                BuildingResourceButton.UnregisterCallback<NavigationSubmitEvent>(BuildingResourceButton_NavigationSubmitEvent, TrickleDown.TrickleDown);
            }
        }

        public void InitializeRequiredResources(List<RequiredResource> requiredResources)
        {
            RequiredResources = requiredResources;
            // Reset all labels
            int totalLabels = 3;
            for (int i=0; i< totalLabels; i++)
            {
                string idLabel = "BuildingResourceLabel_" + i;
                Label resourcelabel = resourceBuildingTemplate.Query<Label>(idLabel);

                if (resourcelabel != null)
                {
                    resourcelabel.text = "";
                }
            } 

            buildingRequiredResources = new Dictionary<ResourceType, RequiredResourceElementUI>();

            for (int i=0; i< requiredResources.Count; i++)
            {
                if (!buildingRequiredResources.ContainsKey(requiredResources[i].Type))
                { 
                    RequiredResourceElementUI elementUI = new RequiredResourceElementUI();
                    elementUI.Title = requiredResources[i].Type.ToString() + ":" + "\n";
                    
                    int playerAmount = 0;
                    int totalRequired = requiredResources[i].Amount;
                    elementUI.Value = missingResourceTextColor + playerAmount.ToString() + "</color>/" + totalRequired.ToString();

                    elementUI.TotalRequired = requiredResources[i].Amount;

                    string idLabel = "BuildingResourceLabel_" + i;
                    Label resourcelabel = resourceBuildingTemplate.Query<Label>(idLabel);
                    if (resourcelabel != null)
                    {
                        elementUI.ValueLabel = resourcelabel;
                        elementUI.ValueLabel.text = elementUI.Title + elementUI.Value;  
                    }

                    buildingRequiredResources.Add(requiredResources[i].Type, elementUI);
                }
                
            }
        }

        public void UpdateRequiredResource(ResourceType type, int playerAmount)
        {
            if (buildingRequiredResources.ContainsKey(type))
            {
                if (playerAmount < buildingRequiredResources[type].TotalRequired)
                {
                    buildingRequiredResources[type].Value = missingResourceTextColor + playerAmount.ToString() + "</color>/" + buildingRequiredResources[type].TotalRequired.ToString();
                }
                else
                {
                    buildingRequiredResources[type].Value = enoughResourceTextColor + playerAmount.ToString() + "</color>/" + buildingRequiredResources[type].TotalRequired.ToString();
                }

                buildingRequiredResources[type].ValueLabel.text = buildingRequiredResources[type].Title + buildingRequiredResources[type].Value;
            }

        }

    }
}
