using PatternToolbox.DataStructures.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Pipeline
{
    public abstract class Operation<ContextType, DataType> : IOperation<ContextType, DataType>
    {
        public abstract Result<bool, OperationFailureEnum> Execute(ContextType ctx, ref DataType data);
    }
}
