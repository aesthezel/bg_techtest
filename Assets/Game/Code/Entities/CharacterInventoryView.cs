using System.Collections.Generic;
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
        
        private readonly List<IShopSlot<ClothItem>> configuredSlots = new();
        private readonly List<IShopSlot<ClothItem>> selectedSlots = new();
        
        private string SelectedItemsDialog => $"You have selected {selectedSlots.Count} items";
        
        public void Configure(Character character)
        {
            Character = character;
            
            foreach (var item in Character.CurrentItems)
            {
                var slotInstance = Instantiate(itemSlot, inventoryContainer).GetComponent<IShopSlot<ClothItem>>();
                
                if (slotInstance == null) return;
                
                slotInstance.Configure(item, ModifyInventory);
                configuredSlots.Add(slotInstance);
            }
            
            closeButton.onClick.AddListener(Close);
            toggleInventoryButton.onClick.AddListener(OpenInventory);
            //tradeButton.onClick.AddListener(Trade);
            clearButton.onClick.AddListener(ClearInventory);
            UpdateInventoryView();
        }

        private void OpenInventory()
        {
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
        }
        
        private void ModifyInventory(IShopSlot<ClothItem> slot)
        {
            if (inventoryState == InventoryState.Locker)
            {
                selectedSlots.Add(slot);
                Character.ChangeCloth(slot.Item.SelectedCategory, slot.Item.SelectedLabel);
                ClearInventory();
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
        
        private void ClearInventory()
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