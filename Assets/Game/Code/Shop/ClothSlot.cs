using System;
using Game.Code.Shop.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Code.Shop
{
    public class ClothSlot : MonoBehaviour, IShopSlot<ClothItem>
    {
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private TMP_Text itemName;
        [SerializeField]
        private TMP_Text itemValue;
        [SerializeField] 
        private Button selectionButton;
        [SerializeField] 
        private Color normalButtonColor;
        [SerializeField]
        private Color selectedButtonColor;
        
        public ClothItem Item { get; private set; }

        private bool selected;

        public void Configure(ClothItem item, Action<IShopSlot<ClothItem>> onClick)
        {
            Item = item;
            itemImage.sprite = item.Sprite;
            itemName.text = string.IsNullOrEmpty(item.Name) ? item.name : item.Name; // If is null return the ScriptableObject base name
            itemValue.text = item.Cost.ToString();
            
            selectionButton.onClick.AddListener(() => PerformClick(onClick));
        }

        private void PerformClick(Action<IShopSlot<ClothItem>> callback)
        {
            selected = !selected;
            selectionButton.image.color = selected ? selectedButtonColor : normalButtonColor;
            callback?.Invoke(this);
        }

        public void SoldOut()
        {
            gameObject.SetActive(false);
        }
        
        public void ForceReset()
        {
            selected = false;
            selectionButton.image.color = normalButtonColor;
            gameObject.SetActive(true);
        }
    }
}