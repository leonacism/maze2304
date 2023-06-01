using Cysharp.Threading.Tasks;
using Scenes.InGame.Items;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.InGame.HUD
{
    public class HUDInventoryView : MonoBehaviour
    {
        public GameObject itemSlotPrefab;

        private InventoryLogic inventory;

        private GameObject[] itemSlots;

        private void Awake()
        {
            inventory = GameObject.FindObjectOfType<InventoryLogic>();
        }

        private void Start()
        {
            itemSlots = new GameObject[inventory.ItemStacks.Length];

            var token = this.GetCancellationTokenOnDestroy();

            for (int i = 0; i < inventory.MaxItemCount; i++)
            {
                var itemSlot = Instantiate(itemSlotPrefab, transform);

                var width = itemSlot.GetComponent<RectTransform>().rect.width;
                itemSlot.transform.position += new Vector3((width * (i - 0.5f * (inventory.MaxItemCount - 1))), 0f, 0f);

                itemSlots[i] = itemSlot;

                UpdateSlot(i, token).SuppressCancellationThrow();
            }
        }

        private async UniTask UpdateSlot(int itemIndex, CancellationToken token)
        {
            var itemSlot = itemSlots[itemIndex];
            var backgroundHighlightImage = itemSlot.transform.Find("ItemBackgroundHighlightImage").GetComponent<Image>();
            var itemThumbnailImage = itemSlot.transform.Find("ItemThumbnailImage").GetComponent<Image>();
            var countText = itemSlot.transform.Find("ItemCountText").GetComponent<TextMeshProUGUI>();

            await UniTask.Yield(token);

            while (true)
            {
                var stack = inventory.ItemStacks[itemIndex];

                backgroundHighlightImage.fillAmount = inventory.CurrentItemIndex == itemIndex? 1f - inventory.ItemConsumptionProgress : 1f;
                backgroundHighlightImage.enabled = inventory.CurrentItemIndex == itemIndex;
                itemThumbnailImage.sprite = stack?.ThumbnailSprite;
                itemThumbnailImage.enabled = stack != null;
                countText.text = stack?.Count > 1 ? stack.Count.ToString() : "";

                await UniTask.Yield(token);
            }
        }
    }
}
