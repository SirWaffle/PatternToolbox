using PatternToolbox.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.EventRouter
{
    public class UnorderedEventRouter<EventBaseClassType, EventArgsBaseClassType, EventHandlerType> : 
        EventRouter<EventBaseClassType, EventArgsBaseClassType, EventHandlerType, object>,
        IEventRouter<EventBaseClassType, EventArgsBaseClassType>
        where EventBaseClassType : class
        //where EventArgsBaseClassType : class 
        where EventHandlerType : IEventHandler<EventBaseClassType, EventArgsBaseClassType>
    {
        public UnorderedEventRouter() : base() 
        { }

        protected override IDictionary CreateHandlerDictionary()
        {
            return new Dictionary<object, IEventHandler<EventBaseClassType, EventArgsBaseClassType>>();
        }

        protected Dictionary<object, IEventHandler<EventBaseClassType, EventArgsBaseClassType>> Cast(IDictionary dict)
        {
            return (dict as Dictionary<object, IEventHandler<EventBaseClassType, EventArgsBaseClassType>>)!;
        }

        protected override void OnAddHandler(IDictionary dict, EventBaseClassType ev, object owner, IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            Cast(dict).Add(owner, handler);
        }

        protected override void OnRemoveHandler(IDictionary dict, EventBaseClassType ev, object owner, IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            Cast(dict).Remove(owner);
        }

        protected override void OnRaiseEvent(IDictionary dict, EventBaseClassType ev, ref EventArgsBaseClassType arg1)
        {
            foreach (var handler in Cast(dict).Values)
            {
                if(handler.HandleEvent(ev, ref arg1) == true)
                {
                    logger.Log(LogLevel.Trace, "Event was consumed! {0}, {1}", ev.ToString(), arg1.GetType().Name);
                    break;
                }
            }
        }
    }
}
