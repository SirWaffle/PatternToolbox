using PatternToolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.EventRouter
{
    public abstract class EventRouter<EventBaseClassType, EventArgsBaseClassType, EventHandlerType, DictKey> : 
        IEventRouter<EventBaseClassType, EventArgsBaseClassType>
        where EventBaseClassType : class 
        //where EventArgsBaseClassType : class 
        where EventHandlerType: IEventHandler<EventBaseClassType, EventArgsBaseClassType>
    { 
        protected Dictionary<EventBaseClassType, System.Collections.IDictionary> eventToHandlerList = new();
        protected List<IEventHandler<EventBaseClassType, EventArgsBaseClassType>> fallbackHandlers = new();
        protected List<IEventHandler<EventBaseClassType, EventArgsBaseClassType>> allEventsHandlers = new();
        protected Logger logger;

        public EventRouter()
        {
            logger = Logger.CreateLogger(GetType().Name);
        }


        protected abstract System.Collections.IDictionary CreateHandlerDictionary();
        protected abstract void OnRaiseEvent(System.Collections.IDictionary dict, EventBaseClassType ev, ref EventArgsBaseClassType arg1);
        protected abstract void OnAddHandler(System.Collections.IDictionary dict, EventBaseClassType ev, object owner, IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler);
        protected abstract void OnRemoveHandler(System.Collections.IDictionary dict, EventBaseClassType ev, object owner, IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler);



        public virtual void RegisterHandler(EventBaseClassType ev, object owner, IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            System.Collections.IDictionary? handlers;
            if (!eventToHandlerList.TryGetValue(ev, out handlers))
            {
                handlers = CreateHandlerDictionary();
                eventToHandlerList.Add(ev, handlers);
            }

            OnAddHandler(handlers, ev, owner, handler);
            logger.Log(LogLevel.Trace, "Registering event handler-> event: {0}, owner: {1}, handler: {2}", ev, owner, handler);
        }

        public virtual void UnregisterHandler(EventBaseClassType ev, object owner, IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            System.Collections.IDictionary? handlers;
            if (!eventToHandlerList.TryGetValue(ev, out handlers))
            {
                logger.Log(LogLevel.Warning, "Failed to remove event handler, it was not registered. event: {0}, owner: {1}, handler: {2}", ev, owner, handler);
                return;
            }

            OnRemoveHandler(handlers, ev, owner, handler);
            logger.Log(LogLevel.Trace, "Removing event handler-> event: {0}, owner: {1}, handler: {2}", ev, owner, handler);
        }

        public void RegisterFallbackHandler(IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            if (fallbackHandlers.Contains(handler))
            {
                logger.Log(LogLevel.CriticalError, "Fallback event handler already exists! {0}", handler);
            }
            else
            {
                fallbackHandlers.Add(handler);
            }
        }

        public void UnregisterFallbackHandler(IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            if (!fallbackHandlers.Remove(handler))
            {
                logger.Log(LogLevel.CriticalError, "Tried to unregister a fallback event handler, but it wasn't in the registered list! {0}", handler);
            }
        }

        public void RegisterAllEventsHandler(IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            if (allEventsHandlers.Contains(handler))
            {
                logger.Log(LogLevel.CriticalError, "Same ALL event handler registerd twice! {0}", handler);
            }
            else
            {
                allEventsHandlers.Add(handler);
            }
        }

        public void UnregisterAllEventsHandler(IEventHandler<EventBaseClassType, EventArgsBaseClassType> handler)
        {
            if (!allEventsHandlers.Remove(handler))
            {
                logger.Log(LogLevel.CriticalError, "Tried to unregister an ALL event handler, but it wasn't in the registered list! {0}", handler);
            }
        }        

        public virtual void RaiseEvent(EventBaseClassType ev, ref EventArgsBaseClassType arg1)
        {
            bool raised = false;
            System.Collections.IDictionary? handlers;
            if (eventToHandlerList.TryGetValue(ev, out handlers))
            {
                logger.Log(LogLevel.Trace, "Raising event-> event: {0}, arg1: {1}", ev, arg1!);
                OnRaiseEvent(handlers, ev, ref arg1);
                raised = true;
            }
            else
            {
                if (fallbackHandlers.Count > 0)
                {
                    raised = true;
                    logger.Log(LogLevel.Trace, "calling fallback handlers for event: {0}, handler: {1}", ev, fallbackHandlers);
                    foreach (var handler in fallbackHandlers)
                    {
                        handler.HandleEvent(ev, ref arg1);
                    }
                }
            }

            if (allEventsHandlers.Count > 0)
            {
                raised = true;
                logger.Log(LogLevel.Trace, "calling the 'all event types handlers' for event: {0}, handler: {1}", ev, fallbackHandlers);
                foreach (var handler in allEventsHandlers)
                {
                    handler.HandleEvent(ev, ref arg1);
                }
            }

            if(raised == false)
            {
                logger.Log(LogLevel.Trace, "Raising event-> No Listeners! event: {0}, arg1: {1}", ev, arg1!);
            }
            return;
        }
    }
}
