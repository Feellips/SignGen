using System;
using System.Collections.Generic;
using System.Threading;

namespace SignGen.Library
{
    public class WorkerPool<I, O> : IDisposable where I : class where O : class
    {
        private readonly Queue<Worker<I, O>> freeWorkers;

        private bool disposedValue;

        public WorkerPool(Func<I, O> handler, int threadCount)
        {
            if (threadCount < 1) throw new ArgumentException($"{nameof(threadCount)} can't be lower than 1.");

            freeWorkers = new Queue<Worker<I, O>>(threadCount);

            for (int i = 0; i < threadCount; i++)
            {
                freeWorkers.Enqueue(new Worker<I, O>(handler));
            }
        }

        public void EnqueueResource(I data)
        {
            Worker<I, O> worker;


        }

        public O DequeueResult()
        {
            O result;

           
            return result;
        }




        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~CustomThreadPool()
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
