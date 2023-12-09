using Game.Code.Shop;
using UnityEngine;

namespace Game.Code.Entities
{
    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Game/Entities/Character Settings", order = 0)]
    public class CharacterSettings : ScriptableObject
    {
        public int StartingCredits;
        public ClothItem[] StartingItems;
    }
}