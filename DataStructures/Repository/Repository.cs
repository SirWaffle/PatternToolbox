using PatternToolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Repository
{
    public class Repository<T>
    {
        public IEnumerable<String> ItemNames { get { return items.Keys; } }

        protected Dictionary<String, T> items = new();

        private readonly Logger logger;

        public Repository()
        { 
            logger = Logger.CreateLogger("Repository<" + typeof(T).Name + ">");
        }

        public virtual void RegisterItem(Type item)
        {
            IRepositoryItem<T>? i = (IRepositoryItem<T>?)Activator.CreateInstance(item);
            if (i == null)
            {
                logger.Log( LogLevel.CriticalError, "Failed to get IRepositoryItem interface from type {0}", item.Name);
                return;
            }
            RegisterInstance(i);
        }

        public virtual void RegisterInstance(T item)
        {
            IRepositoryItem<T>? i = (IRepositoryItem<T>?)item;
            if (i == null)
            {
                logger.Log(LogLevel.CriticalError, "Failed to get IRepositoryItem interface from type {0}", typeof(T).Name);
                return;
            }
            RegisterInstance(i);
        }

        public virtual void RegisterInstance(IRepositoryItem<T> item)
        {
            String itemName = item.RepositoryName;

            if (items.ContainsKey(itemName))
            {
                logger.Log(LogLevel.CriticalError, "item already registered! {0}", item.RepositoryName);
                return;
            }

            if (item is T itemT)
            {
                if (itemT != null)
                {
                    items.Add(itemName, itemT);
                    logger.Log(LogLevel.Trace, "Added {0} with type {1}", itemName, item.GetType().Name);
                    return;
                }

            }

            logger.Log(LogLevel.CriticalError, "item could not be cast to type when registering!  {0}", itemName);
        }

        public virtual T? Get(string name)
        {
            if (!items.TryGetValue(name, out T? item))
            {
                logger.Log(LogLevel.CriticalError, "item is not in repository! -> {0}", name);
            }

            return item;
        }

        public virtual U? Get<U>(string name) where U : class
        {
            if (!items.TryGetValue(name, out T? item))
            {
                logger.Log(LogLevel.CriticalError, "item is not in repository! -> {0}", name);
            }

            return item as U;
        }

        public List<U?> Get<U>(List<string> names) where U : class
        {
            List<U?> resolved = new();
            names.ForEach(s => resolved.Add(Get<U>(s)));
            return resolved;
        }

    }
}
