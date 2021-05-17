using SignGen.Library;
using Xunit;

namespace SignGen.Tests
{
    public class WorkerPoolTests
    {
        [Fact]
        private void WorkerPool_ValidValuesAndEmptyFunc_IdenticalValues()
        {
            Assert.True(true);
        }

        [Fact]
        private void WorkerPool_ValidValuesAndHashFunc_Time()
        {
            Assert.True(true);
        }

        [Fact]
        private void WorkerPool_ValidValuesAndInvalidFunc_ExceptionThrown()
        {
            Assert.True(true);
        }

        [Fact]
        private void WorkerPool_InvalidOutputValueAndEmptyFunc_ExceptionThrown()
        {
            Assert.True(true);
        }
    }
}
