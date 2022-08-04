using System;
using System.Diagnostics.CodeAnalysis;

namespace PatternToolbox.DataStructures.EventRouter
{
    public interface IEventHandler<EvtType, EventArg1>
        where EvtType : class
        //where EventArgs : class
    {
        public Type EventType { get; }
        bool ShouldHandleEvent(EvtType ev, ref EventArg1 arg1);
        bool HandleEvent(EvtType ev, ref EventArg1 arg1);
    }
}