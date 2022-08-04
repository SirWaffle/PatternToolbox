using PatternToolbox.DataStructures.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Pipeline
{
    public class Pipeline<ContextType, DataType> : IOperation<ContextType, DataType>
    {
        protected List<IOperation<ContextType, DataType>> Operations { get; } = new List<IOperation<ContextType,DataType>>();
        
        
        public virtual void Append(IOperation<ContextType, DataType> op)
        {
            Operations.Add(op);
        }

        public void Clear()
        {
            Operations.Clear();
        }

        public virtual Result<bool, OperationFailureEnum> Execute(ContextType ctx, ref DataType data)
        {           
            Result<bool, OperationFailureEnum> res = Result<bool, OperationFailureEnum>.Success(true);
            foreach (IOperation<ContextType, DataType> op in Operations)
            {
                res = op.Execute(ctx, ref data);
                if(res.IsFail)
                {
                    return res;
                }
            }

            return res;
        }
    }
}
