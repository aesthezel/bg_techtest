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
        private Character _character;
        
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

        private string SelectedItemsDialog => $"You have selected {selectedSlots.Count} items";
        private int GetTotalCreditsAmount => selectedSlots.Count == 0 ? 0 : selectedSlots.Sum(slot => slot.Item.Cost);

        private readonly List<IShopSlot<ClothItem>> configuredSlots = new();
        private readonly List<IShopSlot<ClothItem>> selectedSlots = new();
        #endregion

        #region Methods
        private void Open(Character character)
        {
            _character = character;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private void Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
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
            selectedItemsCredits.text = $"<color=yellow>{GetTotalCreditsAmount}c</color>";
            payButton.enabled = selectedSlots.Count > 0;
        }

        private void Pay()
        {
            if (selectedSlots.Count == 0) return;
            
            foreach (var selectedSlot in selectedSlots)
            {
                selectedSlot.SoldOut();
            }
            
            var elementsToSold = configuredSlots.Intersect(selectedSlots); // Intersect selected items to be erased from Shop Database
            configuredSlots.RemoveAll(elementsToSold.Contains);
            
            selectedSlots.Clear();
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
            
            payButton.onClick.AddListener(Pay);
        }
        #endregion
    }
}   