using Core;
using Scenes.InGame.Items;
using Scenes.InGame.Player;
using System;
using UnityEngine;

namespace Scenes.InGame.Goal
{
    public class Goal : Interactable
    {
        public float VerticalBobFrequency = 1f;

        public float BobbingAmplitude = 0.2f;

        public float RotationSpeed = 30f;

        private Vector3 initialPosition;

        public int RequiredNumOfKeys = 5;

        private int currentNumOfKeys;

        public override string InteractionEventName
        {
            get
            {
                return RequiredNumOfKeys > currentNumOfKeys? "鍵を入れる" : "";
            }
        }

        private void Awake()
        {
            initialPosition = transform.position;
        }

        private void Update()
        {
            transform.position = initialPosition + BobbingAmplitude * Mathf.Sin(VerticalBobFrequency * Time.time) * Vector3.up;

            transform.RotateAround(initialPosition, Vector3.down, RotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (currentNumOfKeys >= RequiredNumOfKeys)
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player)
                {
                    EventManager.Broadcast(new GameOver(true));
                }
            }
            else
            {
                Debug.Log("Key item required!");
            }
        }

        public override void OnInteract(InventoryLogic inventory)
        {
            var currentStack = inventory.ItemStacks[inventory.CurrentItemIndex];

            if (currentStack != null && currentStack.ItemName == ItemKind.Key)
            {
                var residue = Math.Min(RequiredNumOfKeys - currentNumOfKeys, currentStack.Count);

                currentStack.Count -= residue;
                currentNumOfKeys += residue;

                inventory.UpdateItemInHand();
            }
            else
            {
                if (currentNumOfKeys < RequiredNumOfKeys) Debug.Log("Key item required!");
            }
        }
    }    
}
