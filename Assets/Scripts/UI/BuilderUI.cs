using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CityBuilder
{
    public class BuilderUI : MonoBehaviour
    {
        public delegate void OnViewInitializedDelegate();
        public OnViewInitializedDelegate OnViewInitialized;

        [SerializeField] private UIDocument uiDocument;

        public ResourceBuilderUI PotionShopBuilding { get; private set; }
        public ResourceBuilderUI RestTentBuilding { get; private set; }

        public void Initialize()
        {
            SetReferences();

            OnViewInitialized?.Invoke();
        }      

        private void SetReferences()
        {
            PotionShopBuilding = new ResourceBuilderUI();            
            TemplateContainer potionShopBuildingTemplate = uiDocument.rootVisualElement.Query<TemplateContainer>("PotionShopBuildingTemplate");
            PotionShopBuilding.SetResource(BuildingType.PotionShop,potionShopBuildingTemplate);            

            RestTentBuilding = new ResourceBuilderUI();
            TemplateContainer restTentBuildingTemplate = uiDocument.rootVisualElement.Query<TemplateContainer>("RestTentBuildingTemplate");
            RestTentBuilding.SetResource(BuildingType.RestTent, restTentBuildingTemplate);
        }

        public void InitializeResources(BuildingType type, List<RequiredResource> RequiredResources)
        {
            switch (type)
            {
                case BuildingType.PotionShop:
                    PotionShopBuilding.InitializeRequiredResources(RequiredResources);
                    break;
                case BuildingType.RestTent:
                    RestTentBuilding.InitializeRequiredResources(RequiredResources);
                    break;
            }
        }

        public void UpdateRequiredResources(ResourceType type, int playerResourceAmount)
        {
            PotionShopBuilding.UpdateRequiredResource(type, playerResourceAmount);
            RestTentBuilding.UpdateRequiredResource(type, playerResourceAmount);
        }

        private void OnDisable()
        {
            if (PotionShopBuilding != null)
            {
                PotionShopBuilding.DesRegisterEvents();
            }

            if (RestTentBuilding != null)
            {
                RestTentBuilding.DesRegisterEvents();
            }
        }

    }
}
