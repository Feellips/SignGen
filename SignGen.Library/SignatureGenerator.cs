using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using SignGen.Library.ProducerConsumer;
using SignGen.Library.ThreadAgents;

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

        public string GetHashBlock(ByteBlock byteBlock)
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

        public void Start(int threads)
        {
            try
            {
                using (var pool = new WorkerPool<ByteBlock, string>(GetHashBlock, threads))
                {
                    var producer = new Producer<ByteBlock, string>(ByteBlock.GetByteBlock, input, pool, blockSize);
                    var consumer = new Consumer<ByteBlock, string>(ByteBlock.ToByteArray, pool, output);

                    producer.Start();
                    consumer.Start();

                    producer.WaitCompletion();
                    consumer.WaitCompletion();
                }
            }
            catch (Exception) { throw; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    input.Dispose();
                    output.Dispose();
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
