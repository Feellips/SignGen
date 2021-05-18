using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using SignGen.Library;
using Xunit;

namespace SignGen.Tests
{
    public class SignatureGeneratorTests
    {
        [Fact]
        private void SignatureGenerator_SingleThreadedAndValidValues_TimeCheck()
        {
            var input = File.Open("Files/SingleThread/singleInput.txt", FileMode.Open, FileAccess.Read);
            var output = File.Open("Files/SingleThread/singleOutput.txt", FileMode.Open, FileAccess.Write);

            using var sigen = new SignatureGenerator(input, output);

            sigen.Start(1);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndValidValues_TimeCheck()
        {
            var input = File.Open("Files/MultiThread/multiInput.txt", FileMode.Open, FileAccess.Read);
            var output = File.Open("Files/MultiThread/multiOutput.txt", FileMode.Open, FileAccess.Write);

            using var sigen = new SignatureGenerator(input, output);

            sigen.Start(8);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndInvalidInputStream_ThrowException()
        {
            var ex = default(Exception);

            var input = File.Open("Files/MultiThread/multiInput.txt", FileMode.Open, FileAccess.Read);
            var output = File.Open("Files/MultiThread/multiOutput.txt", FileMode.Open, FileAccess.Write);

            using var sigen = new SignatureGenerator(input, output);

            var thread = new Thread(() =>
             {
                 try { sigen.Start(8); }
                 catch (Exception e) { ex = e; }
             });

            thread.Start();

            Thread.Sleep(1);

            input.Dispose();
            thread.Join();

            Assert.True(ex != null);
        }

        [Fact]
        private void SignatureGenerator_MultiThreadedAndInvalidOutputStream_ThrowException()
        {
            var ex = default(Exception);

            var input = File.Open("Files/MultiThread/multiInput.txt", FileMode.Open, FileAccess.Read);
            var output = File.Open("Files/MultiThread/multiOutput.txt", FileMode.Open, FileAccess.Write);

            using var sigen = new SignatureGenerator(input, output);

            var thread = new Thread(() =>
            {
                try { sigen.Start(8); }
                catch (Exception e) { ex = e; }
            });

            thread.Start();

            Thread.Sleep(1);

            output.Dispose();
            thread.Join();

            Assert.True(ex != null);
        }
    }
}
