using PatternToolbox.Logging;
//using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Pool
{
    public enum PooledItemState
    {
        PoolingDisabled,
        None,
        CreatedAndUnused,
        InUse,
        ReturnedToPool,
        Destructed
    }

    //SQUISH-TODO: can check pooledState of items to make sure things arent being grabbed or returned multiple times, or something
    [DataContract]
    public class Pool<T>: IPool where T:class, IPoolableObject<T>, new()
    {
        private ConcurrentBag<T> _freeList = new();

        [DataMember]
        //[JsonProperty]
        private ConcurrentDictionary<T, T> _usedList = new();

        public int MaxCapacity { get; }
        public int GrowStep { get; } = 10;

        protected Logger logger = Logger.CreateLogger<Pool<T>>();

        public Pool(int initialCapacity, int maxCapacity)
        {
            MaxCapacity = maxCapacity;
            Grow(initialCapacity);
        }

        public Type PoolType()
        {
            return typeof(T);
        }

        public void Shutdown()
        {
            foreach(var obj in _usedList.Values)
            {
                obj.PooledState = PooledItemState.Destructed;
                obj.PoolDestruct();
            }

            foreach (var obj in _freeList)
            {
                obj.PooledState = PooledItemState.Destructed;
                obj.PoolDestruct();
            }

            _freeList.Clear();
            _usedList.Clear();
        }

        public void Grow( int amt )
        {
            logger.Log(LogLevel.Debug, "Growing pool {0} from {1} by {2}",typeof(T).Name, _freeList.Count, amt);
            for (int i = 0; i < amt; ++i)
            {
                T obj = new();

                obj.PoolInit(this);
                obj.PooledState = PooledItemState.CreatedAndUnused;

                _freeList.Add(obj);
            }
        }

        public T? Get()
        {
            T? res;
            if(!_freeList.TryTake(out res) )
            {
                Grow(GrowStep);
                if (!_freeList.TryTake(out res))
                {
                    logger.Log(LogLevel.CriticalError, "Failed to create poolable object! pool size {0}", _freeList.Count);
                    return null;
                }
            }

            if (!_usedList.TryAdd(res, res))
            {
                logger.Log(LogLevel.CriticalError, "tried to get an object from the free list, but its already in the used list!");
            }

            res.PooledState = PooledItemState.InUse;
            return res;
        }

        public void Return(T obj)
        {
            if (!_usedList.TryRemove(obj, out _ ))
            {
                logger.Log(LogLevel.CriticalError, "tried to return an object to the pool, but it wasn't in the used list!");
                return;
            }

            obj.PoolReset();
            _freeList.Add(obj);
            obj.PooledState = PooledItemState.ReturnedToPool;
        }

        public void Remove(T obj)
        {
            if(!_usedList.TryRemove(obj, out _))
            {
                if (!_freeList.TryTake(out _))
                {
                    logger.Log(LogLevel.CriticalError, "tried to remove an object from the pool, but it wasn't in the pool at all!");
                    return;
                }
            }

            obj.PooledState = PooledItemState.Destructed;
            obj.PoolDestruct();
        }

        public void AddDeserializedObject(T obj)
        {
            switch(obj.PooledState)
            {    
                // goes to free list
                case PooledItemState.CreatedAndUnused:
                case PooledItemState.ReturnedToPool:
                    _freeList.Add(obj);
                    break;


                case PooledItemState.InUse:
                    if (!_usedList.TryAdd(obj, obj))
                    {
                        logger.Log(LogLevel.CriticalError, "tried to get an object from the free list, but its already in the used list!");
                    }
                    break;

                //Error cases
                case PooledItemState.None:
                case PooledItemState.PoolingDisabled:             
                case PooledItemState.Destructed:
                default:
                    logger.Log(LogLevel.CriticalError, "Error Adding deserialized object to pool {0} but its state is {1}", typeof(T).Name, obj.PooledState);
                    return;

            }

            obj.PoolInit(this);
            logger.Log(LogLevel.Trace, "Adding deserialized object to pool <{0}> for free size {1} and used size {2}", typeof(T).Name, _freeList.Count, _usedList.Count);
        }
    }
}
