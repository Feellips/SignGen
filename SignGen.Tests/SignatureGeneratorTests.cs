using System;
using System.IO;
using System.Threading;
using SignGen.Library;
using SignGen.Library.ThreadAgents.Exceptions;
using Xunit;

namespace SignGen.Tests
{
    public class SignatureGeneratorTests
    {
        #region Fields

        private const string inputSingle = "Files/SingleThread/singleInput.txt";
        private const string inputSingle2 = "Files/SingleThread/singleInput2.txt";
        private const string inputSingle3 = "Files/SingleThread/singleInput3.txt";

        private const string outputSingle = "Files/SingleThread/singleOutput.txt";
        private const string outputSingle2 = "Files/SingleThread/singleOutput2.txt";
        private const string outputSingle3 = "Files/SingleThread/singleOutput3.txt";
        
        private const string inputMulti = "Files/MultiThread/multiInput.txt";
        private const string inputMulti2 = "Files/MultiThread/multiInput2.txt";
        private const string inputMulti3 = "Files/MultiThread/multiInput3.txt";

        private const string outputMulti = "Files/MultiThread/multiOutput.txt";
        private const string outputMulti2 = "Files/MultiThread/multiOutput2.txt";
        private const string outputMulti3 = "Files/MultiThread/multiOutput3.txt";

        #endregion

        [Fact]
        private void SignatureGenerator_SingleThreadedAndValidValues_TimeCheck()
        {
            RunSignatureGenerator(inputSingle, outputSingle, 1, null, null);
            Assert.True(true);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndValidValues_TimeCheck()
        {
            RunSignatureGenerator(inputMulti, outputMulti, 8, null, null);
            Assert.True(true);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndInputStreamInterrupts_ThrowException()
        {
            var action = new Action(() => RunSignatureGenerator(inputMulti2, outputMulti2, 8, InterruptStream, null));
            Assert.Throws<ObjectDisposedException>(action);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndOutputStreamInterrupts_ThrowException()
        {
            var action = new Action(() => RunSignatureGenerator(inputMulti3, outputMulti3, 8, null, InterruptStream));
            Assert.Throws<WorkerStoppedException>(action);
        }

        private void InterruptStream(Stream stream)
        {
            Thread.Sleep(1);
            stream.Dispose();
        }

        private void RunSignatureGenerator(string inputPath,
                                           string outputPath,
                                           int threads,
                                           Action<Stream> action,
                                           Action<Stream> action2)
        {
            var ex = default(Exception);

            var input = File.Open(inputPath, FileMode.Open, FileAccess.Read);
            var output = File.Open(outputPath, FileMode.Open, FileAccess.Write);

            using var sigen = new SignatureGenerator(input, output);

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

            action?.Invoke(input);
            action2?.Invoke(output);

            thread.Join();

            if (ex != default(Exception)) throw ex;
        }
    }
}
