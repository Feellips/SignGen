using System;

namespace SignGen.MultithreadExecution.Workers.Exceptions
{
    public class WorkerStoppedException : Exception
    {
        public override string Message => "Worker stopped";
    }
}
