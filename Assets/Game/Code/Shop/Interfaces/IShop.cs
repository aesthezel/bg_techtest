namespace Game.Code.Shop.Interfaces
{
    public interface IShop<out T>
    {
        public T[] ShopSlotItems { get; }
    }
}