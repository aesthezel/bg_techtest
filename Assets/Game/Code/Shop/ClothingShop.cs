using System;
using System.Collections.Generic;
using System.Linq;
using Game.Code.Shop.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Code.Shop
{
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
        private int GetTotalCreditsAmount => selectedSlots.Count == 0 ? 0 : selectedSlots.Sum(slot => slot.Item.Cost);

        private readonly List<IShopSlot<ClothItem>> configuredSlots = new();
        private readonly List<IShopSlot<ClothItem>> selectedSlots = new();
        #endregion

        #region Methods

        /// <summary>
        /// Opens the specified character on the canvas.
        /// </summary>
        /// <param name="character">The character to be opened.</param>
        private void Open(Character character)
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
        }

        /// <summary>
        /// Modifies the shopping cart by adding or removing a cloth item from the selected slots.
        /// </summary>
        /// <param name="slot">The slot to be modified.</param>
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

        /// <summary>
        /// Updates the shopping cart view.
        /// </summary>
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
            
            var elementsToSold = configuredSlots.Intersect(selectedSlots); // Intersect selected items to be erased from Shop Database
            configuredSlots.RemoveAll(elementsToSold.Contains);
            
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