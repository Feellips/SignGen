using System;

namespace SignGen.ThreadJuggler.Workers.Exceptions
{
    public class WorkerStoppedException : Exception
    {
        public override string Message => "Worker stopped";
    }
}
