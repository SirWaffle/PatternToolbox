using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.EventRouter
{
    public struct EventHandler<EvtType, EventArg1> :
        IEventHandlerPriority,
        IEqualityComparer<EventHandler<EvtType, EventArg1>>, 
        IEventHandler<EvtType, EventArg1> 
        where EvtType : class// where EventArgs : class
    {
        public delegate bool ConditionFunc(EventArg1 args);
        public delegate bool HandlerFunc(ref EventArg1 args);

        public Type EventType { get { return typeof(EvtType); } }

        public ConditionFunc? Condition;
        public HandlerFunc handler;

        public int Priority { get; set; }

        public EventHandler(HandlerFunc handlerFunc, int priority, ConditionFunc? condition = null)
        {
            Condition = condition;
            handler = handlerFunc;
            Priority = priority;
        }

        public EventHandler(HandlerFunc handlerFunc, int priority = 0)
        {
            Condition = null;
            handler = handlerFunc;
            Priority = priority;
        }

        public bool ShouldHandleEvent(EvtType ev, ref EventArg1 arg1)
        {
            if (Condition == null)
                return true;

            return Condition.Invoke(arg1);
        }

        public bool HandleEvent(EvtType ev, ref EventArg1 arg1)
        {
            return handler.Invoke(ref arg1);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }

        public bool Equals(EventHandler<EvtType, EventArg1> x, EventHandler<EvtType, EventArg1> y)
        {
            return x.handler == y.handler && x.Condition == y.Condition;
        }

        public int GetHashCode([DisallowNull] EventHandler<EvtType, EventArg1> obj)
        {
            return obj.GetHashCode();
        }
    }

    /*
    public struct EventHandler<EventType, EventArg1> :
        IEventHandlerPriority,
        IEqualityComparer<EventHandler<EventType, EventArg1>>,
        IEventHandler<EventType, EventArg1>
        where EventType : class// where EventArgs : class
    {
        public Func<EventArg1, bool> func;

        public EventHandler(Func<EventArg1, bool> handlerFunc)
        {
            func = handlerFunc;
        }
        public bool HandleEvent(EventType ev, ref EventArg1 arg1)
        {
            return func.Invoke(arg1);
        }

        public override int GetHashCode()
        {
            return func.GetHashCode();
        }

        public bool Equals(EventHandler<EventType, EventArg1> x, EventHandler<EventType, EventArg1> y)
        {
            return x.func == y.func;
        }

        public int GetHashCode([DisallowNull] EventHandler<EventType, EventArg1> obj)
        {
            return obj.GetHashCode();
        }
    }*/
}
