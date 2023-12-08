using System;

namespace Game.Code.Shop.Interfaces
{
    public interface IShopSlot<T>
    {
        public T Item { get; }
        public void Configure(T item, Action<IShopSlot<T>> OnClick);
        public void SoldOut();
        public void ForceReset();
    }
}