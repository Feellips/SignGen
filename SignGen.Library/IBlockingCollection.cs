using System;
using System.Collections.Generic;
using System.Text;

namespace SignGen.Library
{
    public interface IBlockingCollection<I, out O>
    {
        void Enqueue(I item);
        O Dequeue();
        void CompleteAdding();
    }
}
