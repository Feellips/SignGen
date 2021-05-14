using System;
using System.Collections.Generic;
using System.Threading;

namespace SignGen.Library
{
    public class WorkerPool<I, O> : IDisposable where I : class where O : class
    {
        private readonly BlockingQueueWrapper<Worker<I, O>> freeWorkers;
        private readonly BlockingQueueWrapper<Worker<I, O>> busyWorkers;

        private readonly Func<I, O> handler;
        private readonly int threadCount;

        public bool AnyBusyWorkers => busyWorkers.Count > 0;

        public WorkerPool(Func<I, O> handler, int threadCount)
        {
            if (threadCount < 1) throw new ArgumentException($"{nameof(threadCount)} can't be lower than 1.");

            freeWorkers = new BlockingQueueWrapper<Worker<I, O>>(threadCount);
            busyWorkers = new BlockingQueueWrapper<Worker<I, O>>(threadCount);

            this.handler = handler;
            this.threadCount = threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                freeWorkers.Enqueue(new Worker<I, O>(handler));
            }

        }


        public void EnqueueResource(I data)
        {
            Worker<I, O> worker;



            lock (freeWorkers)
            {
                if (freeWorkers.Count == 0) Monitor.Wait(freeWorkers);

                worker = freeWorkers.Dequeue();

                Monitor.Pulse(freeWorkers);
            }

            worker.FeedData(data);

            lock (busyWorkers)
            {
                busyWorkers.Enqueue(worker);

                Monitor.Pulse(busyWorkers);
            }
        }

        public O DequeueResult()
        {
            Worker<I, O> worker;
            O data;

            lock (busyWorkers)
            {
                if (busyWorkers.Count == 0) Monitor.Wait(busyWorkers);

                worker = busyWorkers.Dequeue();

                Monitor.Pulse(busyWorkers);
            }

            try
            {
                data = worker.Result;
            }
            catch (Exception e)
            {
                throw;
            }

            lock (freeWorkers)
            {
                freeWorkers.Enqueue(worker);
                Monitor.Pulse(freeWorkers);
            }

            return data;
        }



        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Worker<I, O> item in freeWorkers)
                {
                    item.Dispose();
                }  
            }

            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
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
