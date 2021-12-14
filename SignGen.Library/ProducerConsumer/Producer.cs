using System;
using System.IO;
using System.Threading;
using SignGen.Library.ThreadAgents;

namespace SignGen.Library.ProducerConsumer
{
    internal class Producer<TInput, TOutput> : StreamAgent<TInput, TOutput> where TInput : class where TOutput : class
    {
        private readonly Func<int, byte[], TInput> _factory;
        private readonly int _blockSize;

        public Producer(Func<int, byte[], TInput> factory, Stream source, IBlockingQueueWorker<TInput, TOutput> destination, int blockSize) : base(source, destination)
        {
            _thread = new Thread(ProduceItems);

            _factory = factory;
            _blockSize = blockSize;
        }

        private void ProduceItems()
        {
            int bufferSize;
            int id = 0;

            try
            {
                while ((bufferSize = NextBufferSize()) > 0)
                {
                    var dataBlock = GetDataBlock(bufferSize);
                    var byteBlock = ByteArrayToGenericInput(id++, dataBlock);

                    _queueWorker.Enqueue(byteBlock);
                }
                _queueWorker.CompleteAdding();
            }
            catch (Exception e) { exception = e; }

        }
        private int NextBufferSize()
        {
            return _stream.Length - _stream.Position < _blockSize
                ? (int)(_stream.Length - _stream.Position)
                : _blockSize;
        }
        private byte[] GetDataBlock(int bufferSize)
        {
            var buffer = new byte[bufferSize];

            _stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }
        private TInput ByteArrayToGenericInput(int id, byte[] blockBytes) => _factory(id, blockBytes);
    }
}
