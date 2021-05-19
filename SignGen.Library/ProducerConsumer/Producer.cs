using System;
using System.IO;
using System.Threading;
using SignGen.Library.ThreadAgents;

namespace SignGen.Library.ProducerConsumer
{
    internal class Producer<I, O> : StreamAgent<I, O> where I : class where O : class
    {
        private readonly Func<int, byte[], I> factory;

        private readonly int blockSize;

        public Producer(Func<int, byte[], I> factory, Stream source, IBlockingQueueWorker<I, O> destination, int blockSize) : base(source, destination)
        {
            thread = new Thread(ProduceItems);

            this.factory = factory;
            this.blockSize = blockSize;
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

                    collection.Enqueue(byteBlock);
                }
                collection.CompleteAdding();
            }
            catch (Exception e) { exception = e; }

        }
        private int NextBufferSize()
        {
            return stream.Length - stream.Position < blockSize
                ? (int)(stream.Length - stream.Position)
                : blockSize;
        }
        private byte[] GetDataBlock(int bufferSize)
        {
            var buffer = new byte[bufferSize];

            stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }
        private I ByteArrayToGenericInput(int id, byte[] blockBytes) => factory(id, blockBytes);
    }
}
