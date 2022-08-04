using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Factory
{
    public interface IFactoryBuilder<T> where T: class
    { 
        string FactoryName { get; }
        T? Build();// { return Activator.CreateInstance(typeof(T)) as T; }
    }
}
