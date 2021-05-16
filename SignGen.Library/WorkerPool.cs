using System;
using System.Collections.Generic;
using System.Threading;

namespace SignGen.Library
{
    public class WorkerPool<I, O> : IDisposable where I : class where O : class
    {
        private readonly List<Worker<I, O>> workers;

        private readonly IEnumerator<Worker<I, O>> producerEnum;
        private readonly IEnumerator<Worker<I, O>> consumerEnum;

        private readonly Func<I, O> handler;
        private readonly int threadCount;

        private bool isEmpty = true;

        public bool IsEmpty => isEmpty;

        public WorkerPool(Func<I, O> handler, int threadCount)
        {
            if (threadCount < 1) throw new ArgumentException($"{nameof(threadCount)} can't be lower than 1.");

            this.handler = handler;
            this.threadCount = threadCount;


            workers = new List<Worker<I, O>>(threadCount);

            for (int i = 0; i < threadCount; i++)
                workers.Add(new Worker<I, O>(handler));

            producerEnum = workers.GetEnumerator();
            consumerEnum = workers.GetEnumerator();

        }


        public void EnqueueResource(I data)
        {
            if (!producerEnum.MoveNext())
            {
                producerEnum.Reset();
                producerEnum.MoveNext();
            }

            producerEnum.Current.GiveData(data);

        }

        public O DequeueResult()
        {
            O result;

            if (!consumerEnum.MoveNext())
            {
                consumerEnum.Reset();
                consumerEnum.MoveNext();
            }

            try
            {
               result = consumerEnum.Current.RecieveResult();
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }



        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var item in workers)
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
