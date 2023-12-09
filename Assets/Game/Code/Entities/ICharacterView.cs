namespace Game.Code.Entities
{
    public interface ICharacterView
    {
        public Character Character { get; }
        public void Configure(Character character);
    }
}