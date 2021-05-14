using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SignGen.Library
{
    public class SignatureGenerator : IDisposable
    {
        #region Fields

        private readonly Stream input;
        private readonly Stream output;

        private readonly string path;
        private readonly int blockSize;

        private bool disposedValue;

        #endregion

        #region Constructor
        public SignatureGenerator(Stream input, Stream output) : this(input, output, 4096) { }
        public SignatureGenerator(Stream input, Stream output, int blockSize)
        {
            this.input = input;
            this.output = output;
            this.blockSize = blockSize;
        }

        #endregion

        private int GetBufferSize(Stream _fileStream)
        {
            return _fileStream.Length - _fileStream.Position < blockSize
                ? (int)(_fileStream.Length - _fileStream.Position)
                : blockSize;
        }

        private byte[] GetDataBlock(int size)
        {
            var buffer = new byte[size];
            input.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public static string GetHash(byte[] input)
        {
            var hashAlgorithm = MD5.Create();
            var data = hashAlgorithm.ComputeHash(input);
            var sBuilder = new StringBuilder();

            foreach (var item in data)
            {
                sBuilder.Append(item.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public void Start()
        {
            //var pool = new WorkerPool<byte[], string>(GetHash, 8);

            var worker = new Worker<byte[], string>(GetHash);
            var consumerIsDone = false;

            var consumer = new Thread(() =>
            {
                int size;

                while ((size = GetBufferSize(input, blockSize)) > 0)
                {
                    var dataBlock = GetDataBlock(input, size);
                    worker.FeedData(dataBlock);
                }

                consumerIsDone = true;
            });


            var producer = new Thread(() =>
            {
                int counter = 0;
                string item;

                while (!consumerIsDone)
                {
                    counter++;

                    try
                    {
                        item = worker.Result;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    item = $"{counter}. {item}";

                    Console.WriteLine(item);
                }

            });


            consumer.Start();
            producer.Start();

            consumer.Join();
            producer.Join();

            worker.Dispose();




            return;

            var consumer = new Thread(() =>
            {
                int size;

                while ((size = GetBufferSize(input)) > 0)
                {
                    var dataBlock = GetDataBlock(size);
                    pool.EnqueueResource(dataBlock);
                }

                consumerIsDone = true;
            });


            var producer = new Thread(() =>
            {
                int counter = 0;
                string item;

                while (!consumerIsDone || pool.AnyBusyWorkers)
                {
                    counter++;

                    try
                    {
                        item = pool.DequeueResult();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    item = $"{counter}. {item}";
                        
                    //Console.WriteLine(item);
                }

            });


            consumer.Start();
            producer.Start();

            consumer.Join();
            producer.Join();

            pool.Dispose();
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
        // ~MultithreadedSignatureGenerator()
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
