using PatternToolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Factory
{
    public class BuilderFactory<U, T> where U: class, IFactoryBuilder<T> 
    {
        public IEnumerable<String> ItemNames { get { return items.Keys; } }

        protected Dictionary<String, IFactoryBuilder<T>> items = new();

        private Logger logger = Logger.CreateLogger< BuilderFactory<U,T> >();

        public virtual void RegisterItem(Type item)
        {
            IFactoryBuilder<T>? i = (IFactoryBuilder<T>?)Activator.CreateInstance(item);
            if (i == null)
            {
                logger.Log( LogLevel.CriticalError, "Failed to get IFactoryItem interface from type {0}", item.Name);
                return;
            }
            RegisterItem(i);
        }

        public virtual void RegisterItem(IFactoryBuilder<T> item)
        {
            String itemName = item.FactoryName;

            if (items.ContainsKey(itemName))
            {
                logger.Log(LogLevel.CriticalError, "item already registered! {0}", itemName);
                return;
            }

            items.Add(itemName, item);
            logger.Log(LogLevel.Trace, "Added {0} with type {1}", itemName, item.GetType().Name);
        }

        public virtual T? Create(string name)
        {
            IFactoryBuilder<T>? itemBuilder;
            if (!items.TryGetValue(name, out itemBuilder) || itemBuilder == null)
            {
                logger.Log(LogLevel.CriticalError, "item is not in factory! -> {0}", name);
            }

            return itemBuilder!.Build();
            //return Activator.CreateInstance(itemType!) as U;
        }
    }
}
