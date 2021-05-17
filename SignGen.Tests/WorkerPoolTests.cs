using SignGen.Library;
using System.Threading;
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
            Assert.True(false);
        }

        [Fact]
        private void WorkerPool_ValidValuesAndInvalidFunc_ExceptionThrown()
        {
            Assert.True(false);
        }

        [Fact]
        private void WorkerPool_InvalidOutputValueAndEmptyFunc_ExceptionThrown()
        {
            Assert.True(false);
        }
    }
}
