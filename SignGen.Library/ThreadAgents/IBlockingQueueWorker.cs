using System;

namespace SignGen.Library.ThreadAgents
{
    public interface IBlockingQueueWorker<TInput, TOutput> : IDisposable where TInput : class where TOutput : class
    {
        void Enqueue(TInput item);
        TOutput Dequeue();
        void CompleteAdding();
    }
}
