using System;
using System.Collections.Generic;

namespace SignGen.Library.ThreadAgents
{
    public class WorkerPool<I, O> : IBlockingQueueWorker<I, O> where I : class where O : class
    {
        #region Fields

        private readonly List<Worker<I, O>> workers;

        private readonly IEnumerator<Worker<I, O>> producerEnum;
        private readonly IEnumerator<Worker<I, O>> consumerEnum;

        private bool isDisposed;

        #endregion

        #region Constructor
        public WorkerPool(Func<I, O> handler, int threadCount)
        {
            if (threadCount < 1) throw new ArgumentException($"{nameof(threadCount)} can't be lower than 1.");

            workers = new List<Worker<I, O>>(threadCount);

            for (int i = 0; i < threadCount; i++)
                workers.Add(new Worker<I, O>(handler));

            producerEnum = workers.GetEnumerator();
            consumerEnum = workers.GetEnumerator();
        }

        #endregion

        public void Enqueue(I data)
        {
            CheckDisposed();

            var worker = GetNextWorker(producerEnum);

            try
            {
                worker.Enqueue(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public O Dequeue()
        {
            CheckDisposed();

            O result;

            var worker = GetNextWorker(consumerEnum);

            try
            {
                result = worker.Dequeue();
            }
            catch (Exception)
            {
                throw;
            }

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
            CheckDisposed();

            var worker = GetNextWorker(producerEnum);
            worker.CompleteAdding();
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    foreach (var item in workers)
                    {
                        item.Dispose();
                    }

                    producerEnum.Dispose();
                    consumerEnum.Dispose();
                }

                isDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        private void CheckDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion

    }
}
