using System;
using Game.Code.Shop.Attributes;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Game.Code.Entities
{
    [Serializable]
    public struct CharacterPartsReference
    {
        public SpriteResolver SpriteResolver;
        public string Category;
    }
}