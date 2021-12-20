using System;
using System.IO;
using System.Threading;
using SignGen.ThreadJuggler.Workers;

namespace SignGen.ThreadJuggler.IO
{
    public class Consumer<TInput, TOutput> : StreamAgent<TInput, TOutput> where TInput : class where TOutput : class
    {
        private readonly Func<TOutput, byte[]> _converter;

        public Consumer(Func<TOutput, byte[]> converter,
                        IBlockingQueueWorker<TInput, TOutput> source,
                        Stream destination)
                        : base(destination, source)
        {
            if (destination.CanWrite == false) throw new ArgumentException($"Can't write to {nameof(destination)}");
            
            _converter = converter;
            AgentThread = new Thread(ConsumeItems);
        }

        private void ConsumeItems()
        {
            TOutput item;

            try
            {
                while ((item = QueueWorker.Dequeue()) != null)
                {
                    var byteBlock = GenericOutputToByteArray(item);

                    ActiveStream.Write(byteBlock, 0, byteBlock.Length);
                }
            }
            catch (Exception e)
            {
                ThreadException = e;
            }
            finally
            {
                QueueWorker.Dispose();
            }
        }
        
        private byte[] GenericOutputToByteArray(TOutput output) => _converter(output);
    }
}
