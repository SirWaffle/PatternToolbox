namespace PatternToolbox.DataStructures.Factory
{
    public interface ITypeFactory: IFactory
    {
        void RegisterItem(string itemName, Type item);
    }
}