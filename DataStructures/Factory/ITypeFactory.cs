namespace PatternToolbox.DataStructures.Factory
{
    public interface ITypeFactory
    {
        IEnumerable<string> ItemNames { get; }
        U? Create<U>(string name) where U : class;
        void RegisterItem(string itemName, Type item);
    }
}