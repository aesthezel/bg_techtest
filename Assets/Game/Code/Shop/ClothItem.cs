using System.Collections.Generic;
using Game.Code.Attributes;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Game.Code.Shop
{
    [CreateAssetMenu(fileName = "Clothe Item", menuName = "Game/Shop/Clothe Item", order = 0)]
    public class ClothItem : ScriptableObject
    {
        private readonly string[] ifNullMessage = {"Add an a SpriteLibraryAsset first"};

        [field: SerializeField] 
        public SpriteLibraryAsset ItemSpriteLibrary { get; private set; }

        [EnumerableDropdown("ItemCategories")]
        public string SelectedCategory;
        
        [EnumerableDropdown("ItemLabels")]
        public string SelectedLabel;

        [field: SerializeField] 
        public string Name { get; private set; }

        [field: SerializeField] 
        public int Cost { get; private set; }
        
        [field: SerializeField]
        public bool CanBeSold { get; private set; }

        public Sprite Sprite => ItemSpriteLibrary.GetSprite(SelectedCategory, SelectedLabel);

        public IEnumerable<string> ItemCategories => ItemSpriteLibrary == null ? ifNullMessage : ItemSpriteLibrary.GetCategoryNames();
        public IEnumerable<string> ItemLabels => ItemSpriteLibrary == null ? ifNullMessage : ItemSpriteLibrary.GetCategoryLabelNames(SelectedCategory);
    }
}