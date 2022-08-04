using PatternToolbox.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Pool
{
    [DataContract]
    public class MultiPool
    {
        [DataMember] //[JsonProperty]
        ConcurrentDictionary<Type, IPool > pools = new();

        protected Logger logger;

        public MultiPool()
        {
            logger = Logger.CreateLogger<MultiPool>();
        }

        public void Shutdown()
        {
            foreach (var p in pools.Values)
            {
                p.Shutdown();
            }

            pools.Clear();
        }

        private Pool<T>? GetTypedPool<T>() where T : class, IPoolableObject<T>, new()
        {
            IPool? pool;
            if (!pools.TryGetValue(typeof(T), out pool))
            {
                //create new pool
                logger.Log(LogLevel.Debug, "Creating new pool for {0}", typeof(T).Name);

                logger.Log(LogLevel.Stub, "Need to provide a good way to set pool sizes and inital capacity");
                pool = new Pool<T>(10, 200);
                pools.TryAdd(typeof(T), pool);
            }

            Pool<T>? typedPool = pool as Pool<T>;
            return typedPool;
        }

        public T? Get<T>() where T:class, IPoolableObject<T>, new()
        {
            Pool<T>? typedPool = GetTypedPool<T>();

            T? obj =  typedPool!.Get();
            if(obj == null)
            {
                logger.Log(LogLevel.CriticalError, "Failed to create poolable item {0}!", typeof(T).Name);
            }

            return obj;
        }

        // this method is for deserilized pooled objects that need to be added to the pool
        public void AddDeserializedObject<T>(T obj) where T : class, IPoolableObject<T>, new()
        {
            Pool<T>? typedPool = GetTypedPool<T>();

            typedPool!.AddDeserializedObject(obj);
        }

    }
}
