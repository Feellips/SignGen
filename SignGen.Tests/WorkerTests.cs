using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using SignGen.Library;
using SignGen.Library.ThreadAgents;
using Xunit;
using Xunit.Abstractions;

namespace SignGen.Tests
{
    public class WorkerTests
    {
        [Fact]
        private void Worker_ValidValuesAndEmptyFunc_IdenticalValues()
        {
            var isInOrder = true;

            var workerPool = new Worker<string, string>((str) => str);

            var prod = new Thread(() =>
            {
                for (int i = 0; i < 999_999; i++)
                {
                    workerPool.Enqueue(i.ToString());
                }
                workerPool.CompleteAdding();
            });

            var cons = new Thread(() =>
            {
                for (int i = 0; i < 999_999; i++)
                {
                    var item = workerPool.Dequeue();

                    if (item == null) return;

                    if (item != i.ToString())
                        isInOrder = false;
                }
            });

            prod.Start();
            cons.Start();

            prod.Join();
            cons.Join();

            Assert.True(isInOrder);
        }

        [Fact]
        private void Worker_ValidValuesAndInvalidFunc_ExceptionThrown()
        {
            var inOrder = true;

            var worker = new Worker<string, string>((str) => throw new Exception());
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
                catch (Exception e)
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

            Assert.True(ex != null);

        }
    }
}
