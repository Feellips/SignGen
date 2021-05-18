using System;
using System.Threading;
using SignGen.Library.BlockingCollections;
using SignGen.Library.ThreadAgents.Exceptions;

namespace SignGen.Library.ThreadAgents
{
    public class Worker<I, O> : IBlockingQueueWorker<I, O> where I : class where O : class
    {
        #region Fields

        private readonly Func<I, O> handler;
        private readonly Thread workerThread;

        private readonly object inLocker;
        private readonly object outLocker;

        private readonly BlockingQueueWrapper<I> workToDo;
        private readonly BlockingQueueWrapper<O> doneWork;

        private readonly SemaphoreSlim limiter;

        private volatile Exception exception;
        private bool isDisposed;

        #endregion

        #region Constructor
        public Worker(Func<I, O> handler)
        {
            inLocker = new object();
            outLocker = new object();

            limiter = new SemaphoreSlim(8);

            workToDo = new BlockingQueueWrapper<I>();
            doneWork = new BlockingQueueWrapper<O>();

            this.handler = handler;
            this.workerThread = new Thread(Consume);
            this.workerThread.Start();
        }

        #endregion

        public void Enqueue(I data)
        {
            CheckDisposed();

            if (!workerThread.IsAlive) throw new WorkerStoppedException();

            limiter.Wait();

            lock (inLocker)
            {
                workToDo.Enqueue(data);
                Monitor.Pulse(inLocker);
            }
        }
        public O Dequeue()
        {
            O result;

            lock (outLocker)
            {
                while (doneWork.Count == 0) Monitor.Wait(outLocker);

                result = doneWork.Dequeue();

                Monitor.Pulse(outLocker);
            }

            limiter.Release();

            if (exception != null) throw exception;

            return result;
        }
        public void CompleteAdding()
        {
            Enqueue(null);
        }
        private void Consume()
        {
            I input;
            O result = null;

            while (true)
            {
                lock (inLocker)
                {
                    if (workToDo.Count == 0) Monitor.Wait(inLocker);

                    input = workToDo.Dequeue();
                }

                lock (outLocker)
                {
                    try
                    {
                        if (input != null)
                            result = handler(input);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    finally
                    {
                        doneWork.Enqueue(result);
                    }

                    Monitor.Pulse(outLocker);
                }

                if (result == null) return;

                result = null;
            }

        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (workerThread.IsAlive)
                    {
                        CompleteAdding();
                        workerThread.Join();
                    }
                    limiter.Dispose();
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
