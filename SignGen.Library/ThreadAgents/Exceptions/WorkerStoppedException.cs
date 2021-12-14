using System;

namespace SignGen.Library.ThreadAgents.Exceptions
{
    public class WorkerStoppedException : Exception
    {
        public override string Message => "Worker stopped";
    }
}
