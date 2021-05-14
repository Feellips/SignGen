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

        private readonly Func<I, O> func;
        private readonly Thread workerThread;

        private readonly object lockerData = new object();
        private readonly object lockerResult = new object();

        private volatile I data;
        private volatile O result;

        private volatile Exception ex;
        private bool disposedValue;

        private bool killSelf;

        public Exception Ex => ex;

        public O Result
        {
            get
            {
                lock (lockerResult)
                {
                    Monitor.Wait(lockerResult);

                    if (ex != null)
                        throw ex;

                    return result;
                }
            }
        }

        #endregion

        #region Constructor
        public Worker(Func<I, O> func)
        {
            this.func = func;
            this.workerThread = new Thread(Consume);
            this.workerThread.Start();
        }

        #endregion

        private void Consume()
        {
            I dat;

            while (true)
            {
                bool lockerResultAcquired = false;

                lock (lockerData)
                {
                    if (data == null) Monitor.Wait(lockerData);

                    if (killSelf) return;

                    dat = data;

                }

                Monitor.Enter(lockerResult, ref lockerResultAcquired);

                try
                {
                    result = func.Invoke(dat);
                }
                catch (Exception e)
                {
                    ex = e;
                }
                finally
                {
                    if (Monitor.IsEntered(lockerResult))
                    {
                        Monitor.Pulse(lockerResult);
                        Monitor.Exit(lockerResult);
                    }
                }
            }

        }



        public void FeedData(I data)
        {
            lock (lockerData)
            {
                this.data = data;
                Monitor.Pulse(lockerData);
            }

        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (workerThread.IsAlive)
                    {
                        killSelf = true;
                        lock (lockerData) Monitor.Pulse(lockerData);
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
