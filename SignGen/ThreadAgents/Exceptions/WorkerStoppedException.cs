using System;

namespace SignGen.ThreadAgents.Exceptions
{
    public class WorkerStoppedException : Exception
    {
        public override string Message => "Worker stopped";
    }
}
