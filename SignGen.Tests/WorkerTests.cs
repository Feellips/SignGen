using System;
using System.Threading;
using SignGen.Library.ThreadAgents;
using Xunit;

namespace SignGen.Tests
{
    public class WorkerTests
    {
        [Fact]
        private void Worker_ValidValuesAndEmptyFunc_IdenticalValues()
        {
            Assert.True(RunWorker((str)=>str));
        }

        [Fact]
        private void Worker_ValidValuesAndInvalidFunc_ExceptionThrown()
        {
            Assert.Throws<Exception>(() => RunWorker((str) => throw new Exception("test")));
        }

        private bool RunWorker(Func<string, string> func)
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
