using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SignGen.Library.ProducerConsumer;
using SignGen.Library.ThreadAgents;

namespace SignGen.Library
{
    public class SignatureGenerator : IDisposable
    {
        #region Fields

        private readonly Stream _input;
        private readonly Stream _output;

        private readonly int _blockSize;

        private bool _disposedValue;

        #endregion

        #region Constructor

        public SignatureGenerator(Stream input, Stream output) : this(input, output, 4096)
        {
        }

        public SignatureGenerator(Stream input, Stream output, int blockSize)
        {
            _input = input;
            _output = output;
            _blockSize = blockSize;
        }

        #endregion

        private string GetHashBlock(ByteBlock byteBlock)
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
            using (var pool = new WorkerPool<ByteBlock, string>(GetHashBlock, threads))
            {
                var producer = new Producer<ByteBlock, string>(ByteBlock.GetByteBlock, _input, pool, _blockSize);
                var consumer = new Consumer<ByteBlock, string>(ByteBlock.ToByteArray, pool, _output);

                producer.Start();
                consumer.Start();

                producer.WaitCompletion();
                consumer.WaitCompletion();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _input.Dispose();
                    _output.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}