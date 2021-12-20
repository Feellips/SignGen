using System;
using System.Collections.Generic;

namespace SignGen.ThreadJuggler.Workers
{
    public class WorkerPool<TInput, TOutput> : IBlockingQueueWorker<TInput, TOutput> where TInput : class where TOutput : class
    {
        private readonly List<Worker<TInput, TOutput>> _workers;

        private readonly IEnumerator<Worker<TInput, TOutput>> _producerEnum;
        private readonly IEnumerator<Worker<TInput, TOutput>> _consumerEnum;

        private bool _isDisposed;

        public WorkerPool(Func<TInput, TOutput> handler, int threadCount)
        {
            if (threadCount < 1) throw new ArgumentException($"{nameof(threadCount)} can't be lower than 1.");

            _workers = new List<Worker<TInput, TOutput>>(threadCount);

            for (int i = 0; i < threadCount; i++)
                _workers.Add(new Worker<TInput, TOutput>(handler));

            _producerEnum = _workers.GetEnumerator();
            _consumerEnum = _workers.GetEnumerator();
        }

        public void Enqueue(TInput data)
        {
            CheckDisposed();

            var worker = GetNextWorker(_producerEnum);

            worker.Enqueue(data);
        }
        
        public TOutput Dequeue()
        {
            CheckDisposed();

            TOutput result;

            var worker = GetNextWorker(_consumerEnum);

            result = worker.Dequeue();

            return result;
        }
        
        private Worker<TInput, TOutput> GetNextWorker(IEnumerator<Worker<TInput, TOutput>> workerEnumerator)
        {
            if (workerEnumerator.MoveNext() == false)
            {
                workerEnumerator.Reset();
                workerEnumerator.MoveNext();
            }

            return workerEnumerator.Current;
        }
        
        public void CompleteAdding()
        {
            CheckDisposed();

            var worker = GetNextWorker(_producerEnum);
            worker.CompleteAdding();
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed == false)
            {
                if (disposing)
                {
                    foreach (var item in _workers)
                    {
                        item.Dispose();
                    }

                    _producerEnum.Dispose();
                    _consumerEnum.Dispose();
                }

                _isDisposed = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        
        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion

    }
}
