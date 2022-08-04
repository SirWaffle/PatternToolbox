using PatternToolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Factory
{
    public class TypeFactory<T> where T: class
    {
        public IEnumerable<String> ItemNames { get { return items.Keys; } }

        protected Dictionary<String,Type> items = new();

        private Logger logger = Logger.CreateLogger<TypeFactory<T>>();

        public virtual void RegisterItem(String itemName, Type item)
        {
            if( item != typeof(T) && item.IsSubclassOf(typeof(T)) == false )
            {
                logger.Log(LogLevel.CriticalError, "item type {0} is not a or is not a subclass of type {1}", item.FullName!, typeof(T).FullName!);
                return;
            }

            if (items.ContainsKey(itemName))
            {
                logger.Log(LogLevel.CriticalError, "item already registered! {0}", itemName);
                return;
            }

            items.Add(itemName, item);
            logger.Log(LogLevel.Trace, "Added {0} with type {1}", itemName, typeof(T).Name);
        }

        public virtual T? Create(string name)
        {
            Type? itemType;
            if (!items.TryGetValue(name, out itemType) || itemType == null)
            {
                logger.Log(LogLevel.CriticalError, "item is not in factory! -> {0}", name);
            }

            T? item = Activator.CreateInstance(itemType!) as T;
            return item;
        }

        public virtual U? Create<U>(string name) where U: class
        {            
            Type? itemType;
            if (!items.TryGetValue(name, out itemType) || itemType == null)
            {
                logger.Log(LogLevel.CriticalError, "item is not in factory! -> {0}", name);
            }

            U? item = Activator.CreateInstance(itemType!) as U;
            return item;
        }
    }
}
