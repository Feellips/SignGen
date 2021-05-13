using System;
using System.Collections.Generic;
using System.Threading;

namespace SignGen.Library
{
    public class CustomThreadPool : IDisposable
    {
        private readonly List<Thread> workers;
        private readonly Action action;

        private bool disposedValue;

        public CustomThreadPool(Action action, int threadCount)
        {
            if (threadCount < 1) throw new ArgumentException($"{nameof(threadCount)} can't be lower than 1.");

            this.action = action;

            workers = new List<Thread>(threadCount);
        }

        public void Begin()
        {
            for (int i = 0; i < workers.Capacity; i++)
            {
                var worker = new Thread(ConsumeResource) { Name = $"Thread {i}" };
                workers.Add(worker);
                worker.Start();
            }
        }

        private static void Handler(Exception exception)
        {
            Console.WriteLine(exception);
        }

        private void ConsumeResource()
        {
            while (true)
            {

            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //  workers.ForEach(thread => Enqueue(null));
                    workers.ForEach(thread => thread.Join());
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
