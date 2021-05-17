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

        public static string GetHash(ByteBlock byteBlock)
        {
            var hashAlgorithm = SHA256.Create();
            var data = hashAlgorithm.ComputeHash(byteBlock.Block);
            var sBuilder = new StringBuilder();

            foreach (var item in data)
            {
                sBuilder.Append(item.ToString("x2"));
            }

            return $"{byteBlock.ID}. {sBuilder} \r\n";
        }

        public void Start()
        {
            var pool = new WorkerPool<ByteBlock, string>(GetHash, 8);

            var producer = new Producer<ByteBlock, string>(ByteBlock.GetByteBlock, input, pool, blockSize);
            var consumer = new Consumer<ByteBlock, string>(ByteBlock.ByteBlockToByteArray, pool, output);

            producer.Start();
            consumer.Start();

            consumer.WaitCompletion();
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
