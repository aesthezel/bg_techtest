using System.Collections.Generic;
using System.Linq;
using Game.Code.Shop;
using Game.Code.Shop.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Code.Entities
{
    public class CharacterInventoryView : MonoBehaviour, ICharacterView
    {
        public enum InventoryState
        {
            Locker,
            Trading
        }

        [SerializeField] 
        private InventoryState inventoryState = InventoryState.Locker;
        [SerializeField] 
        private CanvasGroup canvasGroup;
        [SerializeField]
        private RectTransform inventoryContainer;
        [SerializeField]
        private TMP_Text selectedItems;
        [SerializeField]
        private TMP_Text totalItemsCost;
        [SerializeField]
        private Button tradeButton;
        [SerializeField]
        private Button clearButton;
        [SerializeField]
        private Button closeButton;

        [SerializeField] 
        private Button toggleInventoryButton;
        [SerializeField] 
        private GameObject bottomSection;
        [SerializeField]
        private GameObject itemSlot;
        public Character Character { get; private set; }

        private ClothingShop _clothingShop;
        private int shopPercentFee;
        
        private readonly List<IShopSlot<ClothItem>> configuredSlots = new();
        private readonly List<IShopSlot<ClothItem>> selectedSlots = new();
        
        private int GetTotalCreditsAmount => selectedSlots.Count == 0 ? 0 : selectedSlots.Sum(slot => slot.ReferenceCost);
        
        private string SelectedItemsDialog => $"You have selected {selectedSlots.Count} items";
        
        public void Configure(Character character)
        {
            Character = character;
            Character.OnInteractWithShop += LinkWithShop;
            Character.OnItemsUpdate += UpdateItemDatabase;
            
            foreach (var item in Character.CurrentItems)
            {
                var slotInstance = Instantiate(itemSlot, inventoryContainer).GetComponent<IShopSlot<ClothItem>>();
                
                if (slotInstance == null) return;
                
                slotInstance.Configure(item, ModifyInventory);
                configuredSlots.Add(slotInstance);
            }
            
            closeButton.onClick.AddListener(Close);
            toggleInventoryButton.onClick.AddListener(OpenInventory);
            tradeButton.onClick.AddListener(Trade);
            clearButton.onClick.AddListener(ClearSelected);
            UpdateInventoryView();
        }

        private void UpdateItemDatabase(List<ClothItem> items)
        {
            var itemsToAdd = 0;
            
            for (var i = 0; i < items.Count; i++)
            {
                if (i < configuredSlots.Count)
                {
                    configuredSlots[i].Configure(items[i], ModifyInventory);
                    continue;
                }

                itemsToAdd++;
            }

            if (itemsToAdd <= 0) return;

            for (var i = items.Count - itemsToAdd; i < items.Count; i++)
            {
                var slotInstance = Instantiate(itemSlot, inventoryContainer).GetComponent<IShopSlot<ClothItem>>();
                
                if (slotInstance == null) return;
                slotInstance.Configure(items[i], ModifyInventory);
                configuredSlots.Add(slotInstance);
            }
            
            UpdateInventoryView();
        }

        private void LinkWithShop(ClothingShop shop)
        {
            _clothingShop = shop;
            _clothingShop.OnShopClose += ForceClose;
            
            shopPercentFee = shop.PercentTradeFee;
            OpenTrade();
        }

        private void OpenInventory()
        {
            if (_clothingShop != null)
            {
                if (_clothingShop.IsOpen && canvasGroup.alpha < 1)
                {
                    OpenTrade();
                    return;
                }
            }
            
            inventoryState = InventoryState.Locker;
            if (canvasGroup.alpha < 1)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        private void OpenTrade()
        {
            inventoryState = InventoryState.Trading;
            Open();
        }
        
        private void Open()
        {
            bottomSection.SetActive(inventoryState == InventoryState.Trading);

            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            UpdateInventoryView();
        }

        private void ForceClose()
        {
            inventoryState = InventoryState.Locker;
            _clothingShop.OnShopClose -= ForceClose;
            shopPercentFee = 0;
            Close();
        }        
        
        private void Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        private void UpdateInventoryView()
        {
            selectedItems.text = SelectedItemsDialog;
            tradeButton.enabled = selectedSlots.Count > 0;

            switch (inventoryState)
            {
                case InventoryState.Trading:
                {
                    foreach (var slot in configuredSlots)
                    {
                        if (!slot.Item.CanBeSold)
                        {
                            slot.SoldOut();
                            continue;
                        }
                        
                        float discount = slot.Item.Cost * (shopPercentFee / 100f);
                        var finalPrice = slot.Item.Cost - Mathf.CeilToInt(discount);
                        slot.ChangePrice(finalPrice);
                    }

                    totalItemsCost.text = $"Money to Receive<br><color=yellow>{GetTotalCreditsAmount}c</color>";
                    break;
                }
                case InventoryState.Locker:
                default:
                {
                    foreach (var slot in configuredSlots)
                    {
                        slot.ForceReset();
                    }

                    break;
                }
            }
        }
        
        private void ModifyInventory(IShopSlot<ClothItem> slot)
        {
            if (inventoryState == InventoryState.Locker)
            {
                selectedSlots.Add(slot);
                Character.ChangeCloth(slot.Item.SelectedCategory, slot.Item.SelectedLabel);
                ClearSelected();
            }
            else
            {
                if (selectedSlots.Contains(slot))
                {
                    selectedSlots.Remove(slot);
                }
                else
                {
                    selectedSlots.Add(slot);
                }
                
                UpdateInventoryView();
            }
        }

        private void Trade()
        {
            if (selectedSlots.Count == 0) return;
            if (_clothingShop == null) return;

            var tradeItems = new List<ClothItem>();
            
            foreach (var selectedSlot in selectedSlots)
            {
                selectedSlot.SoldOut();
                tradeItems.Add(selectedSlot.Item);
            }
            
            Character.TradeItems(GetTotalCreditsAmount, tradeItems);
            _clothingShop.GetItems(tradeItems);
            
            selectedSlots.Clear();
            UpdateInventoryView();
        }
        
        private void ClearSelected()
        {
            if (selectedSlots.Count == 0) return;
            
            foreach (var selectedSlot in selectedSlots)
            {
                selectedSlot.ForceReset();
            }
            
            selectedSlots.Clear();
            UpdateInventoryView();
        }
    }
}