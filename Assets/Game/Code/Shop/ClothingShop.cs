using System;
using System.Collections.Generic;
using System.Linq;
using Game.Code.Shop.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Code.Shop
{
    // TODO: ClothingShop must be a Interface -or- Abstract
    public class ClothingShop : MonoBehaviour
    {
        private const string CanBuyColorTag = "green";
        private const string NotEnoughMoneyColorTag = "red";
        
        #region Fields and Properties
        [Header("Shop Settings")]
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField] 
        private Button closeButton;
        [SerializeField]
        private Character character;

        [field: SerializeField, Range(0, 100)]
        public int PercentTradeFee { get; private set; }

        private bool buyerEnoughMoney;
        
        [Header("Item Settings")]
        [SerializeField] 
        private List<ClothItem> itemsToSold;
        [SerializeField]
        private GameObject itemSlot;
        [SerializeField] 
        private RectTransform itemContainer;

        [Header("Pay Settings")] 
        [SerializeField]
        private TMP_Text selectedItems;
        [SerializeField]
        private TMP_Text selectedItemsCredits;
        [SerializeField] 
        private Button payButton;
        [SerializeField] 
        private Button clearButton;

        private string SelectedItemsDialog => $"You have selected {selectedSlots.Count} items";
        private int GetTotalCreditsAmount => selectedSlots.Count == 0 ? 0 : selectedSlots.Sum(slot => slot.ReferenceCost);
        
        public bool IsOpen => canvasGroup.alpha >= 1;

        private readonly List<IShopSlot<ClothItem>> configuredSlots = new();
        private readonly List<IShopSlot<ClothItem>> selectedSlots = new();

        public Action OnShopClose;
        #endregion

        #region Methods
        
        public void Open(Character character)
        {
            this.character = character;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            UpdateShoppingCartView();
        }
        
        private void Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            OnShopClose?.Invoke();
        }
        
        private void ModifyShoppingCart(IShopSlot<ClothItem> slot)
        {
            if (selectedSlots.Contains(slot))
            {
                selectedSlots.Remove(slot);
            }
            else
            {
                selectedSlots.Add(slot);
            }
            
            UpdateShoppingCartView();
        }
        
        private void UpdateShoppingCartView()
        {
            selectedItems.text = SelectedItemsDialog;

            var enoughMoneyColor = character.Credits >= GetTotalCreditsAmount ? CanBuyColorTag : NotEnoughMoneyColorTag;
            selectedItemsCredits.text = $"Total: <color={enoughMoneyColor}>{GetTotalCreditsAmount}c</color>";
            payButton.enabled = selectedSlots.Count > 0;

            buyerEnoughMoney = character.Credits >= GetTotalCreditsAmount;
        }
        
        private void Payout()
        {
            if (selectedSlots.Count == 0 || !buyerEnoughMoney) return;

            var soldItems = new List<ClothItem>();
            
            foreach (var selectedSlot in selectedSlots)
            {
                selectedSlot.SoldOut();
                soldItems.Add(selectedSlot.Item);
            }
            
            character.ObtainItems(GetTotalCreditsAmount, soldItems.ToArray());
            
            selectedSlots.Clear();
            UpdateShoppingCartView();
        }

        private void ClearCart()
        {
            if (selectedSlots.Count == 0) return;
            
            foreach (var selectedSlot in selectedSlots)
            {
                selectedSlot.ForceReset();
            }
            
            selectedSlots.Clear();
            UpdateShoppingCartView();
        }

        public void GetItems(List<ClothItem> items)
        {
            var itemsToAdd = 0;
            
            for (var i = 0; i < items.Count; i++)
            {
                if (i < configuredSlots.Count)
                {
                    configuredSlots[i].Configure(items[i], ModifyShoppingCart);
                    configuredSlots[i].ForceReset();
                    continue;
                }

                itemsToAdd++;
            }

            if (itemsToAdd <= 0) return;

            for (var i = items.Count - itemsToAdd; i < items.Count; i++)
            {
                var slotInstance = Instantiate(itemSlot, itemContainer).GetComponent<IShopSlot<ClothItem>>();
                
                if (slotInstance == null) return;
                slotInstance.Configure(items[i], ModifyShoppingCart);
                configuredSlots.Add(slotInstance);
            }
        }
        #endregion
        
        #region Unity Workflow
        private void Start()
        {
            foreach (var item in itemsToSold)
            {
                var slotInstance = Instantiate(itemSlot, itemContainer).GetComponent<IShopSlot<ClothItem>>();
                
                if (slotInstance == null) return;
                
                slotInstance.Configure(item, ModifyShoppingCart);
                configuredSlots.Add(slotInstance);
            }
            
            closeButton.onClick.AddListener(Close);
            payButton.onClick.AddListener(Payout);
            clearButton.onClick.AddListener(ClearCart);
            UpdateShoppingCartView();
        }
        #endregion
    }
}   