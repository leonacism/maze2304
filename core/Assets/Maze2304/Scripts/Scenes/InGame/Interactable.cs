using Scenes.InGame.Items;
using UnityEngine;

namespace Scenes.InGame
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract string InteractionEventName { get; }

        public abstract void OnInteract(InventoryLogic inventory);
    }
}
