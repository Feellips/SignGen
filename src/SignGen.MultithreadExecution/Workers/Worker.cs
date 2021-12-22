using System;
using System.Threading;
using SignGen.MultithreadExecution.Collections;
using SignGen.MultithreadExecution.Workers.Exceptions;

namespace SignGen.MultithreadExecution.Workers
{
    public class Worker<TInput, TOutput> : IBlockingQueueWorker<TInput, TOutput> where TInput : class where TOutput : class
    {
        private const int WorkLimit = 8;
        
        private readonly SemaphoreSlim _workLimiter;
        private readonly Thread _workerThread;
        private readonly Func<TInput, TOutput> _handler;

        private readonly object _enqueueLocker = new object();
        private readonly object _dequeueLocker = new object();

        private readonly BlockingQueueWrapper<TInput> _workToDo;
        private readonly BlockingQueueWrapper<TOutput> _doneWork;

        private Exception _handlerException;

        private bool _isDisposed;
        private bool _isCompleted;

        public Worker(Func<TInput, TOutput> handler)
        {
            _workLimiter = new SemaphoreSlim(WorkLimit);

            _workToDo = new BlockingQueueWrapper<TInput>();
            _doneWork = new BlockingQueueWrapper<TOutput>();

            _handler = handler;

            _workerThread = new Thread(Consume);
            _workerThread.Start();
        }

        public void Enqueue(TInput data)
        {
            CheckDisposed();

            _workLimiter.Wait();

            if (_workerThread.IsAlive == false || _isCompleted) throw new WorkerStoppedException();

            EnqueueDirectly(data);
        }
        
        private void EnqueueDirectly(TInput data)
        {
            lock (_enqueueLocker)
            {
                _workToDo.Enqueue(data);
                Monitor.Pulse(_enqueueLocker);
            }
        }
        
        public TOutput Dequeue()
        {
            TOutput result;

            lock (_dequeueLocker)
            {
                while (_doneWork.Count == 0) Monitor.Wait(_dequeueLocker);

                result = _doneWork.Dequeue();

                Monitor.Pulse(_dequeueLocker);
            }

            _workLimiter.Release();

            if (_handlerException != null) throw _handlerException;

            return result;
        }
        
        public void CompleteAdding()
        {
            EnqueueDirectly(null);
            _workLimiter.Release();
            _isCompleted = true;
        }
        
        private void Consume()
        {
            TInput input;
            TOutput result = null;

            while (true)
            {
                lock (_enqueueLocker)
                {
                    if (_workToDo.Count == 0) Monitor.Wait(_enqueueLocker);

                    input = _workToDo.Dequeue();
                }

                lock (_dequeueLocker)
                {
                    try
                    {
                        if (input != null)

                            result = _handler(input);
                    }
                    catch (Exception e)
                    {
                        _handlerException = e;
                    }
                    finally
                    {
                        _doneWork.Enqueue(result);
                    }

                    Monitor.Pulse(_dequeueLocker);
                }

                if (result == null) return;

                result = null;
            }

        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed == false)
            {
                if (disposing)
                {
                    if (_workerThread.IsAlive)
                    {
                        CompleteAdding();
                        _workerThread.Join();
                    }
                    _workLimiter.Dispose();
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
