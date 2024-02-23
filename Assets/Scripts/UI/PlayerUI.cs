using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CityBuilder
{
    public class PlayerUI : MonoBehaviour
    {
        public delegate void OnViewInitializedDelegate();
        public OnViewInitializedDelegate OnViewInitialized;

        [SerializeField] private UIDocument uiDocument;

        public Label WoodLabel
        {
            get; private set;
        }

        public Label ManaLabel
        {
            get; private set;
        }

        public Label StoneLabel
        {
            get; private set;
        }

        private VisualElement popupElement;
        private Label popupMessage;

        public void Initialize()
        {
            SetReferences();

            OnViewInitialized?.Invoke();
        }

        private void SetReferences()
        {
            WoodLabel = uiDocument.rootVisualElement.Query<Label>("WoodLabel");
            ManaLabel = uiDocument.rootVisualElement.Query<Label>("ManaLabel");
            StoneLabel = uiDocument.rootVisualElement.Query<Label>("StoneLabel");

            popupElement = uiDocument.rootVisualElement.Query<VisualElement>("PopupMessage");

            if (popupElement != null)
            {
                popupMessage = popupElement.Query<Label>("MessagePopup");
            }
        }

        public void ShowInGameMessage(string message)
        {
            popupElement.visible = true;
            popupMessage.text = message;

            StartCoroutine(HidePopup());
        }

        private IEnumerator HidePopup()
        {
            yield return new WaitForSeconds(2.0f);

            popupElement.visible = false;
        }
    }
}
