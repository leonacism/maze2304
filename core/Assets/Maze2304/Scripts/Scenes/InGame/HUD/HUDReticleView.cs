using Cysharp.Threading.Tasks;
using Scenes.InGame.Items;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.InGame.HUD
{
    public class HUDReticleView : MonoBehaviour
    {
        public Image reticleImage;
        public TextMeshProUGUI interactionEventText;

        private InGameInputManager inputManager;
        private InventoryLogic inventory;

        private void Awake()
        {
            inputManager = GameObject.FindObjectOfType<InGameInputManager>();
            inventory = GameObject.FindObjectOfType<InventoryLogic>();
        }

        private void Update()
        {
            var interactable = inputManager.GetInteractable();

            reticleImage.enabled = interactable != null;
            interactionEventText.text = interactable != null ? interactable.InteractionEventName : "";
        }
    }
}
