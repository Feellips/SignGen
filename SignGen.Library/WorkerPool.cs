using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SignGen.Library
{
    public class WorkerPool<I, O> : IDisposable, IBlockingCollection<I, O> where I : class where O : class
    {
        private readonly List<Worker<I, O>> workers;

        private readonly IEnumerator<Worker<I, O>> producerEnum;
        private readonly IEnumerator<Worker<I, O>> consumerEnum;

        public WorkerPool(Func<I, O> handler, int threadCount)
        {
            if (threadCount < 1) throw new ArgumentException($"{nameof(threadCount)} can't be lower than 1.");

            workers = new List<Worker<I, O>>(threadCount);

            for (int i = 0; i < threadCount; i++)
                workers.Add(new Worker<I, O>(handler));

            producerEnum = workers.GetEnumerator();
            consumerEnum = workers.GetEnumerator();
        }


        public void Enqueue(I data)
        {
            var worker = GetNextWorker(producerEnum);
            worker.Enqueue(data);
        }

        public O Dequeue()
        {
            O result;

            var worker = GetNextWorker(consumerEnum);

            try { result = worker.Dequeue(); }
            catch (Exception) { throw; }

            return result;
        }

        private Worker<I, O> GetNextWorker(IEnumerator<Worker<I, O>> workerEnumerator)
        {
            if (!workerEnumerator.MoveNext())
            {
                workerEnumerator.Reset();
                workerEnumerator.MoveNext();
            }

            return workerEnumerator.Current;
        }

        public void CompleteAdding()
        {
            var worker = GetNextWorker(producerEnum);
            worker.CompleteAdding();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var item in workers)
                {
                    item.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
