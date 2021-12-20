using System;
using System.IO;
using System.Threading;
using SignGen.ThreadJuggler.Workers.Exceptions;
using Xunit;

namespace SignGen.Tests
{
    public class SignatureGeneratorTests
    {
        #region Fields

        private const string InputSingle = "Files/SingleThread/singleInput.txt";

        private const string OutputSingle = "Files/SingleThread/singleOutput.txt";
        
        private const string InputMulti = "Files/MultiThread/multiInput.txt";
        private const string InputMulti2 = "Files/MultiThread/multiInput2.txt";
        private const string InputMulti3 = "Files/MultiThread/multiInput3.txt";

        private const string OutputMulti = "Files/MultiThread/multiOutput.txt";
        private const string OutputMulti2 = "Files/MultiThread/multiOutput2.txt";
        private const string OutputMulti3 = "Files/MultiThread/multiOutput3.txt";

        #endregion

        [Fact]
        private void SignatureGenerator_SingleThreadedAndValidValues_TimeCheck()
        {
            RunSignatureGenerator(InputSingle, OutputSingle, 1, null, null);
            Assert.True(true);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndValidValues_TimeCheck()
        {
            RunSignatureGenerator(InputMulti, OutputMulti, 8, null, null);
            Assert.True(true);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndInputStreamInterrupts_ThrowException()
        {
            var action = new Action(() => RunSignatureGenerator(InputMulti2, OutputMulti2, 8, InterruptStream, null));
            Assert.Throws<ObjectDisposedException>(action);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndOutputStreamInterrupts_ThrowException()
        {
            var action = new Action(() => RunSignatureGenerator(InputMulti3, OutputMulti3, 8, null, InterruptStream));
            Assert.Throws<WorkerStoppedException>(action);
        }

        private void InterruptStream(Stream stream)
        {
            Thread.Sleep(5);
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

            using var sigen = new SignatureGenerator(input, output, 4096);

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
