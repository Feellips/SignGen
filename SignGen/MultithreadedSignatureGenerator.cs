using System;
using System.IO;
using System.Text;
using SignGen.ProducerConsumer;
using SignGen.ThreadAgents;

namespace SignGen
{
    public sealed class MultithreadedSignatureGenerator : IDisposable
    {
        private readonly Stream _input;
        private readonly Stream _output;

        private readonly int _blockSize;

        private readonly HashCalculator _hashCalculator;
        private readonly IByteBlockFactory _byteBlockFactory;

        private bool _disposedValue;

        public MultithreadedSignatureGenerator(Stream input, Stream output) : this(input, output, 4096)
        {
        }

        public MultithreadedSignatureGenerator(Stream input, Stream output, int blockSize)
        {
            if (input.CanRead == false) throw new ArgumentException($"Can't read from {nameof(input)}");
            if (output.CanWrite == false) throw new ArgumentException($"Can't write to {nameof(output)}");
            if (blockSize < 8 || blockSize > 1024 * 10)
                throw new ArgumentException($"{nameof(blockSize)} should be from 8 byte to 10 MGb");

            _input = input;
            _output = output;
            _blockSize = blockSize;

            _hashCalculator = new HashCalculator();
            _byteBlockFactory = new ByteBlockFactory();
        }

        public void Start(int threads)
        {
            using (var pool = new WorkerPool<IByteBlock, string>(GetHashBlock, threads))
            {
                var producer = new Producer<IByteBlock, string>(_byteBlockFactory.Create, _input, pool, _blockSize);
                var consumer = new Consumer<IByteBlock, string>(Encoding.ASCII.GetBytes, pool, _output);

                producer.Start();
                consumer.Start();

                producer.WaitCompletion();
                consumer.WaitCompletion();
            }
        }

        private string GetHashBlock(IByteBlock byteBlock)
        {
            string hashBlock = _hashCalculator.Compute(byteBlock.Data);
            return $"{byteBlock.Id} {hashBlock} {Environment.NewLine}";
        }

        private void Dispose(bool disposing)
        {
            if (_disposedValue == false)
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
        }
    }
}