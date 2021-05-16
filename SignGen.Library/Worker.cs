using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SignGen.Library
{
    public class Worker<I, O> : IDisposable where I : class where O : class
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

        private bool isCompleted;

        public Exception Ex => ex;

        public bool IsCompleted => isCompleted && doneWork.Count == 0 && workToDo.Count == 0;

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
            O result;

            while (true)
            {
                lock (inLocker)
                {
                    if (workToDo.Count == 0) Monitor.Wait(inLocker);

                    if (isCompleted) return;

                    input = workToDo.Dequeue();
                }

                lock (outLocker)
                {
                    try
                    {
                        result = handler(input);
                        doneWork.Enqueue(result);
                    }
                    catch (Exception e)
                    {
                        ex = e;
                        doneWork.Enqueue(null);
                    }

                    Monitor.Pulse(outLocker);
                }

            }

        }


        public O RecieveResult()
        {
            O result;

            lock (outLocker)
            {
                while (doneWork.Count == 0 && !isCompleted) Monitor.Wait(outLocker);

                if (ex != null) throw ex;
                
                if (isCompleted) return null;

                result = doneWork.Dequeue();


                Monitor.Pulse(outLocker);
                limiter.Release();
            }

            return result;
        }

        public void GiveData(I data)
        {
            limiter.Wait();

            lock (inLocker)
            {
                if (isCompleted)
                    throw new InvalidOperationException("Queue already stopped");


                workToDo.Enqueue(data);

                Monitor.Pulse(inLocker);
            }
        }

        public void Stop()
        {
            lock (inLocker)
            {
                isCompleted = true;
                Monitor.Pulse(inLocker);
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
