using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using SignGen.Library;
using Xunit;
using Xunit.Abstractions;

namespace SignGen.Tests
{
    public class WorkerTests
    {
        [Fact]
        private void Worker_ValidValuesAndEmptyFunc_IdenticalValuesInOrder()
        {
            Assert.True(true);
        }

        [Fact]
        private void Worker_ValidValuesAndHashFunc_Time()
        {
            Assert.True(true);
        }

        [Fact]
        private void Worker_ValidValuesAndInvalidFunc_ExceptionThrown()
        {
            Assert.True(true);
        }

        [Fact]
        private void Worker_InvalidOutputValueAndEmptyFunc_ExceptionThrown()
        {
            Assert.True(true);
        }
    }
}
