using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Pool
{
    public interface IPoolableObject<T> where T:class, IPoolableObject<T>, new()
    {
        /// <summary>
        /// Current state of the pooled object
        /// </summary>
        public PooledItemState PooledState { get; set; }

        /// <summary>
        /// called when the pool first constructs the object
        /// </summary>
        /// <param name="pool"></param>
        public void PoolInit(Pool<T> pool);


        /// <summary>
        /// returns the object to the pool
        /// </summary>
        public void PoolReturn();


        /// <summary>
        /// called from the pool when the object has been returned to the pool
        /// this is a good place to remove isntance data that you dont want to leave sitting ont he obejct
        /// while its in the pool
        /// </summary>
        public void PoolReset();


        /// <summary>
        /// called when the pool is removing the obejct and letting it go to GC
        /// clean up stuff here that needs its refs cleared or would cause problems
        /// </summary>
        public void PoolDestruct();
    }
}
