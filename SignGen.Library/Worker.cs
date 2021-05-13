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

        private Func<I, O> func;

        private readonly Thread workerThread;
        private readonly object lockerData = new object();
        private readonly object lockerResult = new object();

        private volatile I data;
        private volatile O result;

        private volatile Exception ex;
        private bool disposedValue;
        private bool ready;

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

        internal bool IsFree()
        {
            throw new NotImplementedException();
        }

        #endregion

        private void Consume()
        {
            while (true)
            {
                lock (lockerData)
                {
                    lock (lockerResult)
                    {
                        result = func.Invoke(data);
                        Monitor.Pulse(lockerResult);
                    }
                    Monitor.Pulse(lockerData);
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


        public void Clear()
        {

        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~Worker()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



    }
}
