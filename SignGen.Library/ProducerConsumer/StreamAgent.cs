using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using SignGen.Library.ThreadAgents;

namespace SignGen.Library.ProducerConsumer
{
    internal abstract class StreamAgent<TInput, TOutput> where TInput : class where TOutput : class
    {
        protected readonly IBlockingQueueWorker<TInput, TOutput> _queueWorker;
        protected readonly Stream _stream;
        protected Thread _thread;
        
        protected Exception exception;

        protected StreamAgent(Stream stream, IBlockingQueueWorker<TInput, TOutput> blockingQueueWorker)
        {
            _stream = stream;
            _queueWorker = blockingQueueWorker;
        }

        public void Start() => _thread.Start();
        public void WaitCompletion()
        {
            if (_thread.IsAlive)
                _thread.Join();

            if (exception != null) throw exception;
        }
    }
}
