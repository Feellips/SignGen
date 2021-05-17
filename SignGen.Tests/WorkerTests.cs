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



        private string testData = "The names";
        private string testData2 = "asdfavaxvv";
        private string testData3 = "The nfasdfasaasdmes";
        private string testData4 = "The nasdfasdfasdvames";
        private string testData5 = "The ncvcxvcxzames";
        private string testData6 = "The nafvadvasdvmes";
        private string testData7 = "The nasdvczames";
        private string testData8 = "The nvcxvzxefaames";
        private string testData9 = "The nasdvcxvzxmes";
        private string testData10 = "The ncvasefames";
        private string testData11 = "The nafasdfvvxczvzmes";

        private volatile string output;

        private Func<string, string> emptyFunc = (arg) => {  return arg; };

        [Fact]
        public void Test() { Assert.True(SimpleTest(testData)); }
        [Fact]
        public void Test2() { Assert.True(SimpleTest(testData2)); }
        [Fact]
        public void Test3() { Assert.True(SimpleTest(testData3)); }
        [Fact]
        public void Test4() { Assert.True(SimpleTest(testData4)); }
        [Fact]
        public void Test5() { Assert.True(SimpleTest(testData5)); }
        [Fact]
        public void Test6() { Assert.True(ReusabilityTest(testData6, testData7)); }
        [Fact]
        public void Test7() { Assert.True(ReusabilityTest(testData7, testData8)); }
        [Fact]
        public void Test8() { Assert.True(ReusabilityTest(testData8, testData9)); }
        [Fact]
        public void Test9() { Assert.True(ReusabilityTest(testData9, testData7)); }
        [Fact]
        public void Test10() { Assert.True(ReusabilityTest(testData10, testData3)); }
        [Fact]
        public void Test11() { Assert.True(ReusabilityTest(testData11, testData2)); }

        [Fact]
        public void SignGeneratorOneThread()
        {
            var worker = new Worker<ByteBlock, string>(SignatureGenerator.GetHash);
            var consumerIsDone = false;


            var path = @"C:\Users\PhedyushinPhilipp\Downloads\!PVNDEMIK Galiv - CHANGZ Music Visualization.mp3";
            var blockSize = 4096;

            using var input = File.Open(path, FileMode.Open, FileAccess.Read);
            using var sMem = new MemoryStream();

            var consumer = new Thread(() =>
            {
                int size;
                int counter = 0;

                while ((size = GetBufferSize(input, blockSize)) > 0)
                {
                    var dataBlock = GetDataBlock(input, size);
                    var byteBlock = new ByteBlock(counter++, dataBlock);
                    worker.Enqueue(byteBlock);
                }

                consumerIsDone = true;
            });


            var producer = new Thread(() =>
            {
                int counter = 0;
                string item;

                while (!consumerIsDone)
                {
                    counter++;

                    try
                    {
                        item = worker.Dequeue();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    item = $"{counter}. {item}";

                    Console.WriteLine(item);
                }

            });


            consumer.Start();
            producer.Start();

            consumer.Join();
            producer.Join();

            worker.Dispose();
        }

        private byte[] GetDataBlock(Stream input, int size)
        {
            var buffer = new byte[size];
            input.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        private int GetBufferSize(Stream _fileStream, int blockSize)
        {
            return _fileStream.Length - _fileStream.Position < blockSize
                ? (int)(_fileStream.Length - _fileStream.Position)
                : blockSize;
        }


        private bool SimpleTest(string str)
        {
            var worker = new Worker<string, string>(emptyFunc);

            worker.Enqueue(str);

            return str == worker.Dequeue();
        }

        private bool ReusabilityTest(string str, string str2)
        {
            var worker = new Worker<string, string>(emptyFunc);

            worker.Enqueue(str);

            bool first = str == worker.Dequeue();

            worker.Enqueue(str2);

            bool second = str2 == worker.Dequeue() ;

            return first && second;
        }


    }
}
