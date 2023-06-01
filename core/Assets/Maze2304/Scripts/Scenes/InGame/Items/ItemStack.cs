using System;
using UnityEngine;

namespace Scenes.InGame.Items
{
    public abstract class ItemStack : Interactable
    {
        public int MaxStackCount = 1;

        public bool Consumable = false;
        public float ConsumeHoldDuration = 1f;

        public Vector3 positionOffset = Vector3.zero;
        public Quaternion rotationOffset = Quaternion.identity;

        public Sprite ThumbnailSprite;

        public int Count = 1;

        public ItemKind ItemName { get; protected set; }

        public override string InteractionEventName
        {
            get
            {
                return "取る";
            }
        }

        public override void OnInteract(InventoryLogic inventory)
        {
            var stacks = inventory.ItemStacks;
            var currentItemIndex = inventory.CurrentItemIndex;

            var existingStacks = Array.FindAll(stacks, existingStack => existingStack?.ItemName.Equals(this.ItemName) ?? false);

            // 同じアイテムの空きスタックを巡回する
            foreach (ItemStack existingStack in existingStacks) if (existingStack.Count < existingStack.MaxStackCount)
            {
                // 入れられる個数を計算する
                var residue = Math.Min(existingStack.MaxStackCount - existingStack.Count, this.Count);

                existingStack.Count += residue;
                this.Count -= residue;
                
                // 他の空きスタックに入り切ったら抜ける
                if (this.Count == 0) break;
            }

            ItemStack poppedStack = null;

            // 取得アイテムが残っている場合
            if (this.Count > 0)
            {
                var currentStack = stacks[currentItemIndex];

                // 所持アイテムと取得アイテムが違う種類なら、現在の所持アイテムと取得アイテムを交換する
                if (currentStack != null && currentStack.ItemName != this.ItemName)
                {
                    poppedStack = currentStack;
                    stacks[currentItemIndex] = this;

                    poppedStack.transform.SetParent(transform.parent);
                    poppedStack.transform.localPosition = transform.position;
                    poppedStack.transform.localRotation = transform.rotation;
                }
                // 所持アイテムが空なら、取得アイテムをスロットの中に入れる
                else if (currentStack is null)
                {
                    stacks[currentItemIndex] = this;
                }
                // 所持アイテムと取得アイテムが同じ種類の場合は交換しない
                else
                {
                }
            }
            else
            {
                // 重複しているオブジェクトを消去
                Destroy(this.gameObject);
            }

            inventory.UpdateItemInHand();

            poppedStack?.gameObject.SetActive(true);
        }

        public abstract void OnConsume();

        public abstract void OnHold();
    }
}
