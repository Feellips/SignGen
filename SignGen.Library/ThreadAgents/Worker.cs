using System;
using System.Threading;
using SignGen.Library.BlockingCollections;
using SignGen.Library.ThreadAgents.Exceptions;

namespace SignGen.Library.ThreadAgents
{
    public class Worker<TInput, TOutput> : IBlockingQueueWorker<TInput, TOutput> where TInput : class where TOutput : class
    {
        #region Fields

        private readonly Func<TInput, TOutput> _handler;
        private readonly Thread _workerThread;

        private readonly object _inLocker = new object();
        private readonly object _outLocker = new object();

        private readonly BlockingQueueWrapper<TInput> _workToDo;
        private readonly BlockingQueueWrapper<TOutput> _doneWork;

        private readonly SemaphoreSlim _limiter;

        private volatile Exception _exception;

        private bool _isDisposed;
        private bool _isCompleted;

        #endregion

        #region Constructor
        public Worker(Func<TInput, TOutput> handler)
        {
            _limiter = new SemaphoreSlim(8);

            _workToDo = new BlockingQueueWrapper<TInput>();
            _doneWork = new BlockingQueueWrapper<TOutput>();

            _handler = handler;
            _workerThread = new Thread(Consume);
            _workerThread.Start();
        }

        #endregion

        public void Enqueue(TInput data)
        {
            CheckDisposed();

            _limiter.Wait();

            if (_workerThread.IsAlive == false || _isCompleted) throw new WorkerStoppedException();

            EnqueueDirectly(data);
        }
        private void EnqueueDirectly(TInput data)
        {
            lock (_inLocker)
            {
                _workToDo.Enqueue(data);
                Monitor.Pulse(_inLocker);
            }
        }
        public TOutput Dequeue()
        {
            TOutput result;

            lock (_outLocker)
            {
                while (_doneWork.Count == 0) Monitor.Wait(_outLocker);

                result = _doneWork.Dequeue();

                Monitor.Pulse(_outLocker);
            }

            _limiter.Release();

            if (_exception != null) throw _exception;

            return result;
        }
        public void CompleteAdding()
        {
            EnqueueDirectly(null);
            _limiter.Release();
            _isCompleted = true;
        }
        private void Consume()
        {
            TInput input;
            TOutput result = null;

            while (true)
            {
                lock (_inLocker)
                {
                    if (_workToDo.Count == 0) Monitor.Wait(_inLocker);

                    input = _workToDo.Dequeue();
                }

                lock (_outLocker)
                {
                    try
                    {
                        if (input != null)
                            result = _handler(input);
                    }
                    catch (Exception e)
                    {
                        _exception = e;
                    }
                    finally
                    {
                        _doneWork.Enqueue(result);
                    }

                    Monitor.Pulse(_outLocker);
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
                    _limiter.Dispose();
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
