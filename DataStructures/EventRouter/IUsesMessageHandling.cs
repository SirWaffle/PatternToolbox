using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.EventRouter
{
    public interface IUsesMessageHandling
    {
        public void RegisterHandlers();
        public void UnregisterHandlers();
    }
}
