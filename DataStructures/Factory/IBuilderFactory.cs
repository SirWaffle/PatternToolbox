namespace PatternToolbox.DataStructures.Factory
{
    public interface IBuilderFactory: IFactory
    {
        void RegisterItem<T>(IFactoryBuilder<T> item);
        void RegisterItem(Type item);
    }
}