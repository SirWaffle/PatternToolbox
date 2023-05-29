using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Factory
{
    public interface IFactory
    {
        IEnumerable<string> ItemNames { get; }
        U? Create<U>(string name) where U : class;
    }
}
