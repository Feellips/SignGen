using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SignGen.Library
{
    internal class Consumer<I, O>
    {
        private readonly Stream destination;
        private readonly Func<O, byte[]> converter;
        private readonly IBlockingCollection<I, O> source;
        private readonly Thread consumerThread;

        public Consumer(Func<O, byte[]> converter, IBlockingCollection<I, O> source, Stream destination)
        {
            this.converter = converter;
            this.source = source;
            this.destination = destination;

            consumerThread = new Thread(() =>
            {
                O item;
                int counter = 0;
                try
                {
                    while (true)
                    {
                        item = source.Dequeue();

                        if (item == null)
                            break;

                        var byteBlock = GenericOutputToByteArray(item);

                        destination.Write(byteBlock, 0, byteBlock.Length);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            });
        }

        public void Start() => consumerThread.Start();
        public void WaitCompletion() => consumerThread.Join();
        public byte[] GenericOutputToByteArray(O output) => converter(output);
        

    }
}
