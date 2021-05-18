using System;
using System.IO;
using System.Threading;
using SignGen.Library.ThreadAgents;

namespace SignGen.Library.ProducerConsumer
{
    internal class Consumer<I, O> : StreamAgent<I, O> where I : class where O : class
    {
        private readonly Func<O, byte[]> converter;

        public Consumer(Func<O, byte[]> converter, IBlockingQueueWorker<I, O> source, Stream destination) : base(destination, source)
        {
            this.converter = converter;
            thread = new Thread(ConsumeItems);
        }

        private void ConsumeItems()
        {
            O item;

            try
            {
                while ((item = collection.Dequeue()) != null)
                {
                    var byteBlock = GenericOutputToByteArray(item);

                    stream.Write(byteBlock, 0, byteBlock.Length);
                }
            }
            catch (Exception e) { exception = e; }
        }

        public byte[] GenericOutputToByteArray(O output) => converter(output);
    }
}
