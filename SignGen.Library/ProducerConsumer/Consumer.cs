using System;
using System.IO;
using System.Threading;
using SignGen.Library.ThreadAgents;

namespace SignGen.Library.ProducerConsumer
{
    internal class Consumer<TInput, TOutput> : StreamAgent<TInput, TOutput> where TInput : class where TOutput : class
    {
        private readonly Func<TOutput, byte[]> _converter;

        public Consumer(Func<TOutput, byte[]> converter,
                        IBlockingQueueWorker<TInput, TOutput> source,
                        Stream destination)
                        : base(destination, source)
        {
            _converter = converter;
            _thread = new Thread(ConsumeItems);
        }

        private void ConsumeItems()
        {
            TOutput item;

            try
            {
                while ((item = _queueWorker.Dequeue()) != null)
                {
                    var byteBlock = GenericOutputToByteArray(item);

                    _stream.Write(byteBlock, 0, byteBlock.Length);
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                _queueWorker.Dispose();
            }
        }
        private byte[] GenericOutputToByteArray(TOutput output) => _converter(output);
    }
}
