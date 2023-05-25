using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Factory
{
    public interface IFactoryBuilder<T>
    { 
        string FactoryName { get; }
        T? Build();
    }
}
