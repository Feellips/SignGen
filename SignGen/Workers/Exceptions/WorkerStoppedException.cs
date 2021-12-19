using System;

namespace SignGen.Workers.Exceptions
{
    public class WorkerStoppedException : Exception
    {
        public override string Message => "Worker stopped";
    }
}
