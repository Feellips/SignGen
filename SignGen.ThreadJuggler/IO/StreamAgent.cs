using System;
using System.IO;
using System.Threading;
using SignGen.ThreadJuggler.Workers;

namespace SignGen.ThreadJuggler.IO
{
    public abstract class StreamAgent<TInput, TOutput> where TInput : class where TOutput : class
    {
        protected readonly IBlockingQueueWorker<TInput, TOutput> QueueWorker;
        protected readonly Stream ActiveStream;

        protected Thread AgentThread;
        protected Exception ThreadException;

        protected StreamAgent(Stream activeStream, IBlockingQueueWorker<TInput, TOutput> queueWorker)
        {
            ActiveStream = activeStream;
            QueueWorker = queueWorker;
        }

        public void Start() => AgentThread.Start();
        
        public void WaitCompletion()
        {
            if (AgentThread.IsAlive)
                AgentThread.Join();

            if (ThreadException != null) throw ThreadException;
        }
    }
}
