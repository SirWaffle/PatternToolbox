using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.EventRouter
{
    public interface IEventRouter<EventBaseClassType, EventArgsBaseClassType>
    {
        void RaiseEvent(EventBaseClassType ev, ref EventArgsBaseClassType arg1);
    }
}
