using PatternToolbox.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.EventRouter
{
    public class OrderedEventRouter<EventBaseClassType, EventArgsBaseClassType, EventHandlerType> :
        EventRouter<EventBaseClassType, EventArgsBaseClassType, EventHandlerType, OrderedEventRouter<EventBaseClassType, EventArgsBaseClassType, EventHandlerType>.DictKey>,
        IEventRouter<EventBaseClassType, EventArgsBaseClassType>
        where EventBaseClassType : class
        //where EventArgsBaseClassType : class 
        where EventHandlerType : IEventHandler<EventBaseClassType, EventArgsBaseClassType>, IEventHandlerPriority
    {

        public struct DictKey
        {
            public object owner;
            public int priority;

            public override int GetHashCode()
            {
                return owner.GetHashCode();
            }
        }

        /// <summary>
        /// Sorts from low to high
        /// </summary>
        private class DictComparer : IComparer<DictKey>
        {
            public int Compare(OrderedEventRouter<EventBaseClassType, EventArgsBaseClassType, EventHandlerType>.DictKey x, OrderedEventRouter<EventBaseClassType, EventArgsBaseClassType, EventHandlerType>.DictKey y)
            {
                if (x.priority == y.priority)
                    return 0;
                else if (x.priority > y.priority)
                    return 1;
                else
                    return -1;
            }
        }

        public OrderedEventRouter() : base()
        { }

        protected override System.Collections.IDictionary CreateHandlerDictionary()
        {
            return new SortedDictionary<DictKey, List<IEventHandler<EventBaseClassType, EventArgsBaseClassType>>>(new DictComparer());
        }

        protected SortedDictionary<DictKey, List<IEventHandler<EventBaseClassType, EventArgsBaseClassType>>> Cast(IDictionary dict)
        {
            return (dict as SortedDictionary<DictKey, List<IEventHandler<EventBaseClassType, EventArgsBaseClassType>>>)!;
        }

        protected override void OnAddHandler(IDictionary dict, EventBaseClassType ev, object owner, IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            DictKey key = new();
            key.owner = owner;
            key.priority = (handler as IEventHandlerPriority)!.Priority;
            
            if( !Cast(dict).TryGetValue(key, out List<IEventHandler<EventBaseClassType, EventArgsBaseClassType>>? list))
            {
                list = new();
                Cast(dict).Add(key, list);
            }

            list.Add(handler);
        }

        protected override void OnRemoveHandler(IDictionary dict, EventBaseClassType ev, object owner, IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            DictKey key = new();
            key.owner = owner;
            key.priority = (handler as IEventHandlerPriority)!.Priority;

            if (Cast(dict).TryGetValue(key, out List<IEventHandler<EventBaseClassType, EventArgsBaseClassType>>? list))
            {
                list.Remove(handler);
            }            
            else
            {
                logger.Log(LogLevel.Warning, "Failed to remove event handler, it was not registered. event: {0}, owner: {1}, handler: {2}", ev, owner, handler);
            }
        }

        protected override void OnRaiseEvent(System.Collections.IDictionary dict, EventBaseClassType ev, ref EventArgsBaseClassType arg1)
        {
            bool consumed = false;
            foreach (KeyValuePair<DictKey, List<IEventHandler<EventBaseClassType, EventArgsBaseClassType>>> kvp in Cast(dict))
            {
                foreach (var handler in kvp.Value)
                {
                    consumed = handler.HandleEvent(ev, ref arg1);
                    if (consumed)
                    {
                        logger.Log(LogLevel.Debug, "Event was consumed! {0}, {1}", ev.ToString(), arg1.GetType().Name);
                        break;
                    }
                }
                if (consumed)
                {
                    logger.Log(LogLevel.Debug, "Event was consumed! {0}, {1}", ev.ToString(), arg1.GetType().Name);
                    break;
                }
            }
        }
    }
}
