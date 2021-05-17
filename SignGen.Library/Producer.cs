using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SignGen.Library
{
    internal class Producer<I, O>
    {
        private readonly Func<int, byte[], I> factory;
        private readonly Stream source;
        private readonly IBlockingCollection<I, O> destination;
        private readonly Thread producerThread;

        private readonly int blockSize;


        public Producer(Func<int, byte[], I> factory, Stream source, IBlockingCollection<I, O> destination, int blockSize)
        {
            this.factory = factory;
            this.source = source;
            this.destination = destination;
            this.blockSize = blockSize;

            producerThread = new Thread(() =>
            {
                int bufferSize;
                int counter = 0;

                while ((bufferSize = NextBufferSize(source)) > 0)
                {
                    var dataBlock = GetDataBlock(bufferSize);

                    var byteBlock = ByteArrayToGenericInput(counter++, dataBlock);

                    destination.Enqueue(byteBlock);
                }

                destination.CompleteAdding();
            });
        }

        private int NextBufferSize(Stream stream)
        {
            return stream.Length - stream.Position < blockSize
                ? (int)(stream.Length - stream.Position)
                : blockSize;
        }
        private byte[] GetDataBlock(int bufferSize)
        {
            var buffer = new byte[bufferSize];

            source.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public void Start() => producerThread.Start();

        private I ByteArrayToGenericInput(int id, byte[] blockBytes)
        {
            return factory(id, blockBytes);
        }

    }
}
