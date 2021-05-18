using System;

namespace SignGen.Library.ThreadAgents
{
    public interface IBlockingQueueWorker<I, O> : IDisposable where I : class where O : class
    {
        void Enqueue(I item);
        O Dequeue();
        void CompleteAdding();
    }
}
