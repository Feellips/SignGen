using System;
using System.Threading;
using SignGen.ThreadJuggler.Workers;
using Xunit;

namespace SignGen.Tests.ThreadJuggler
{
    public class WorkerPoolTests
    {
        [Fact]
        private void WorkerPool_ValidValuesAndEmptyFunc_IdenticalValuesInOrder()
        {
            Assert.True(RunWorkerPool((str)=>str));
        }

        [Fact]
        private void WorkerPool_ValidValuesAndInvalidFunc_ExceptionThrown()
        {
            Assert.Throws<Exception>(() => RunWorkerPool((_) => throw new Exception("test")));
        }

        private bool RunWorkerPool(Func<string, string> func)
        {
            var inOrder = true;

            var worker = new WorkerPool<string, string>(func, 8);
            Exception ex = null;

            var prod = new Thread(() =>
            {
                try
                {
                    for (int i = 0; i < 999_999; i++)
                    {
                        worker.Enqueue(i.ToString());
                    }
                    worker.CompleteAdding();
                }
                catch (Exception)
                {
                    // ignored
                }
            });

            var cons = new Thread(() =>
            {
                string item = null;

                for (int i = 0; i < 999_999; i++)
                {
                    try
                    {
                        item = worker.Dequeue();
                    }
                    catch (Exception e) { ex = e; }

                    if (item == null) return;
                }
            });

            prod.Start();
            cons.Start();

            prod.Join();
            cons.Join();

            if (ex != null) throw ex;

            return inOrder;
        }
    }
}
