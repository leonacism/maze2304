using Cysharp.Threading.Tasks;
using Scenes.InGame;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

namespace Scenes.InGame.Items
{
    public class InventoryLogic : MonoBehaviour
    {
        public int MaxItemCount = 5;

        public float ItemConsumptionProgress { get; private set; }

        public bool ItemConsuming { get; private set; }

        public ItemStack[] ItemStacks { get; private set; }

        public int CurrentItemIndex { get; private set; }

        private InGameInputManager inputManager;

        private GameObject currentItemInHand;

        private void Awake()
        {
            ItemStacks = new ItemStack[MaxItemCount];
            inputManager = GameObject.FindObjectOfType<InGameInputManager>();

            CurrentItemIndex = 0;
        }

        private void Start()
        {
            Observable.EveryUpdate()
                .Select(_ => inputManager.GetInteractInput())
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => inputManager.GetInteractable()?.OnInteract(this));

            Observable.EveryUpdate()
                .Select(_ => inputManager.GetConsumeInput())
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => ConsumeItem(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow());

            Observable.EveryUpdate()
                .Select(_ => inputManager.GetScrollInput())
                .DistinctUntilChanged()
                .Where(scroll => scroll != 0)
                .Subscribe(scroll => ChangeItem(scroll));
        }

        private void Update()
        {
            ItemStacks[CurrentItemIndex]?.OnHold();
        }

        private async UniTask ConsumeItem(CancellationToken token)
        {
            var currentStack = ItemStacks[CurrentItemIndex];

            ItemConsuming = true;

            var elapsedTime = 0f;

            while(
                inputManager.GetConsumeInput() &&
                currentStack != null &&
                currentStack.Consumable &&
                currentStack == ItemStacks[CurrentItemIndex]
            )
            {
                elapsedTime += Time.deltaTime;

                ItemConsumptionProgress = Mathf.Clamp01(elapsedTime / currentStack.ConsumeHoldDuration);

                if (ItemConsumptionProgress == 1)
                {
                    currentStack.OnConsume();
                    currentStack.Count--;

                    UpdateItemInHand();

                    elapsedTime = 0f;
                }

                await UniTask.Yield(token);
            }

            ItemConsumptionProgress = 0f;
            ItemConsuming = false;
        }

        private void ChangeItem(int direction)
        {
            CurrentItemIndex = (CurrentItemIndex + direction + MaxItemCount) % MaxItemCount;

            UpdateItemInHand();
        }

        public void UpdateItemInHand()
        {
            if (currentItemInHand != null)
            {
                currentItemInHand.SetActive(false);
            }
            
            var currentStack = ItemStacks[CurrentItemIndex];
            
            if (currentStack != null)
            {
                if (currentStack.Count > 0)
                {
                    currentItemInHand = currentStack.gameObject;
                    currentItemInHand.SetActive(true);

                    currentItemInHand.transform.SetParent(transform);
                    currentItemInHand.transform.localPosition = currentStack.positionOffset;
                    currentItemInHand.transform.localRotation = currentStack.rotationOffset;
                }
                else
                {
                    Destroy(currentStack.gameObject);
                    ItemStacks[CurrentItemIndex] = null;
                }
            }
        }
    }
}
