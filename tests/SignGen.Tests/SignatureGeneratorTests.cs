using System;
using System.IO;
using System.Threading;
using SignGen.MultithreadExecution.Workers.Exceptions;
using Xunit;

namespace SignGen.Tests
{
    public class SignatureGeneratorTests
    {
        private const int SizeInMb = 100;

        [Fact]
        private void SignatureGenerator_SingleThreadedAndValidValues_TimeCheck()
        {
            RunSignatureGenerator(1, null, null);

            Assert.True(true);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndValidValues_TimeCheck()
        {
            RunSignatureGenerator(8, null, null);

            Assert.True(true);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndInputStreamInterrupts_ThrowException()
        {
            var action = new Action(() => RunSignatureGenerator(8, InterruptStream, null));

            Assert.Throws<ObjectDisposedException>(action);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndOutputStreamInterrupts_ThrowException()
        {
            var action = new Action(() => RunSignatureGenerator(8, null, InterruptStream));
          
            Assert.Throws<WorkerStoppedException>(action);
        }

        private void InterruptStream(Stream stream)
        {
            Thread.Sleep(10);
            stream.Close();
            stream.Dispose();
        }

        private void RunSignatureGenerator(int threads,
                                           Action<Stream> action,
                                           Action<Stream> action2)
        {
            var ex = default(Exception);

            using var fileInputStreamImitator = new MemoryStream();
            using var fileOutputStreamImitator = new MemoryStream();

            byte[] data = new byte[SizeInMb * 1024 * 1024];
            var rng = new Random();
            rng.NextBytes(data);
            fileInputStreamImitator.Write(data, 0, data.Length);
            fileInputStreamImitator.Position = 0;

            using var sigen = new SignatureGenerator(fileInputStreamImitator, fileOutputStreamImitator, 1024);

            var thread = new Thread(() =>
            {
                try
                {
                    sigen.Start(threads);
                }
                catch (Exception e)
                {
                    ex = e;
                }
            });

            thread.Start();

            action?.Invoke(fileInputStreamImitator);
            action2?.Invoke(fileOutputStreamImitator);

            thread.Join();

            if (ex != default(Exception)) throw ex;
        }
    }
}
