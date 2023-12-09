namespace Game.Code.Shop.Interfaces
{
    // TODO: Implement on all shops
    public interface IShop<out T>
    {
        public T[] ShopSlotItems { get; }
    }
}