using System;
using System.IO;
using SignGen.Library;
using System.Threading;
using SignGen.Library.ThreadAgents;
using Xunit;

namespace SignGen.Tests
{
    public class WorkerPoolTests
    {
        [Fact]
        private void WorkerPool_ValidValuesAndEmptyFunc_IdenticalValuesInOrder()
        {
            var isInOrder = true;

            var workerPool = new WorkerPool<string, string>((str) => str, 16);

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
        private void WorkerPool_ValidValuesAndHashFunc_Time()
        {
        

           
        }

        [Fact]
        private void WorkerPool_ValidValuesAndInvalidFunc_ExceptionThrown()
        {
            var inOrder = true;

            var worker = new WorkerPool<string, string>((str) => throw new Exception(), 8);
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
