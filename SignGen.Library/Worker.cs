using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SignGen.Library
{
    public class Worker<I, O> : IBlockingCollection<I, O> where I : class where O : class
    {
        #region Fields

        private readonly Func<I, O> handler;
        private readonly Thread workerThread;

        private readonly object inLocker;
        private readonly object outLocker;

        private readonly BlockingQueueWrapper<I> workToDo;
        private readonly BlockingQueueWrapper<O> doneWork;

        private readonly SemaphoreSlim limiter;

        private volatile Exception ex;
        private bool disposedValue;

        public Exception Ex => ex;

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
                        ex = e;
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

        public void Enqueue(I data)
        {
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

                if (ex != null) throw ex;

                result = doneWork.Dequeue();

                Monitor.Pulse(outLocker);
            }

            limiter.Release();

            return result;
        }

        public void CompleteAdding()
        {
            Enqueue(null);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (workerThread.IsAlive)
                    {
                        CompleteAdding();
                        workerThread.Join();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



    }
}
