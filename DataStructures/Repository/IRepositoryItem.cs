using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Repository
{
    public interface IRepositoryItem<T>
    {
        string RepositoryName { get; }
    }
}
