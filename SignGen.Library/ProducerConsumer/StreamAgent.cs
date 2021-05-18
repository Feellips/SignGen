using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using SignGen.Library.ThreadAgents;

namespace SignGen.Library.ProducerConsumer
{
    internal abstract class StreamAgent<I, O> where I : class where O : class
    {
        protected IBlockingQueueWorker<I, O> collection;

        protected Stream stream;
        protected Thread thread;
        protected Exception exception;

        public StreamAgent(Stream stream, IBlockingQueueWorker<I, O> blockingQueueWorker)
        {
            this.stream = stream;
            this.collection = blockingQueueWorker;
        }

        public void Start() => thread.Start();
        public void WaitCompletion()
        {
            if (thread.IsAlive)
                thread.Join();

            if (exception != null) throw exception;
        }
    }
}
