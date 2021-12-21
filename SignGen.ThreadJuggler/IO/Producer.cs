using System;
using System.IO;
using System.Threading;
using SignGen.MultithreadExecution.Workers;

namespace SignGen.MultithreadExecution.IO
{
    public class Producer<TInput, TOutput> : StreamAgent<TInput, TOutput> where TInput : class where TOutput : class
    {
        private readonly Func<int, byte[], TInput> _factory;
        private readonly int _blockSize;

        public Producer(Func<int, byte[], TInput> factory, 
                        Stream source,
                        IBlockingQueueWorker<TInput, TOutput> destination, 
                        int blockSize) : base(source, destination)
        {
            if (source.CanRead == false) throw new ArgumentException($"Can't read from {nameof(source)}");

            AgentThread = new Thread(ProduceItems);

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

                    QueueWorker.Enqueue(byteBlock);
                }

                QueueWorker.CompleteAdding();
            }
            catch (Exception e)
            {
                ThreadException = e;
            }
        }

        private int NextBufferSize()
        {
            return ActiveStream.Length - ActiveStream.Position < _blockSize
                ? (int) (ActiveStream.Length - ActiveStream.Position)
                : _blockSize;
        }

        private byte[] GetDataBlock(int bufferSize)
        {
            var buffer = new byte[bufferSize];
            ActiveStream.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        private TInput ByteArrayToGenericInput(int id, byte[] blockBytes) => _factory(id, blockBytes);
    }
}