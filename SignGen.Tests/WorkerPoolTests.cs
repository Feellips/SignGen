using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using SignGen.Library;
using Xunit;
using Xunit.Abstractions;

namespace SignGen.Tests
{
    public class WorkerPoolTests
    {

        private string[] input = new string[9] { "63d45cd1-bc6c-43de-b5a9-2e101b2ec08f",
                                                 "bb9efad1-2057-4905-a77c-cfd4f39280fc",
                                                 "60302964-c9db-43c9-b53e-83f797fb9fb7",
                                                 "f6dd889b-f1bd-407f-8f80-202bc833b93b",
                                                 "82df39db-d234-4ac0-bb72-3129df92b191",
                                                 "dd800b15-6fbf-41ff-b034-3971e187b204",
                                                 "a8ac501b-ffc1-4bad-89a1-fcf8a1137520",
                                                 "c54def4a-9ece-4b8d-952f-96634b0d1c76",
                                                 "39639780-a53f-4e08-b07e-11e3d5e09aea" };



        private Func<string, string> emptyFunc = (arg) => { return arg; };

        [Fact]
        private void SimpleTest()
        {
            var failed = false;
            var workerPool = new WorkerPool<string, string>(emptyFunc, 1);

            var producer = new Thread(() =>
            {
                foreach (var item in input)
                {
                    workerPool.EnqueueResource(item);
                }
            });


            var consumer = new Thread(() =>
            {
                foreach (var item in input)
                {
                    var consumed = workerPool.DequeueResult();
                    if (consumed != item) 
                        failed = true;
                }
            });

            consumer.Start();
            producer.Start();

            producer.Join();
            consumer.Join();

            Assert.False(failed);
        }

        [Fact]
        private void Test123()
        {
            Start();
        }

        private int GetBufferSize(Stream _fileStream, int blockSize)
        {
            return _fileStream.Length - _fileStream.Position < blockSize
                ? (int)(_fileStream.Length - _fileStream.Position)
                : blockSize;
        }

        private byte[] GetDataBlock(Stream input, int size)
        {
            var buffer = new byte[size];
            input.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public static string GetHash(byte[] input)
        {
            var hashAlgorithm = MD5.Create();
            var data = hashAlgorithm.ComputeHash(input);
            var sBuilder = new StringBuilder();

            foreach (var item in data)
            {
                sBuilder.Append(item.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public void Start()
        {
            var pool = new WorkerPool<byte[], string>(GetHash, 8);
            var consumerIsDone = false;

            var path = @"C:\ProgramData\Autodesk\Inventor 2021\Content Center\Libraries\AI2021_Inventor ISO.idcl";
            var blockSize = 4096;

            using var input = File.Open(path, FileMode.Open, FileAccess.Read);
            using var sMem = new MemoryStream();

            var consumer = new Thread(() =>
            {
                int size;

                while ((size = GetBufferSize(input, blockSize)) > 0)
                {
                    var dataBlock = GetDataBlock(input, size);
                    pool.EnqueueResource(dataBlock);
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
                        item = pool.DequeueResult();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    item = $"{counter}. {item}";
                }

            });


            consumer.Start();
            producer.Start();

            consumer.Join();
            producer.Join();

            pool.Dispose();

            Assert.True(true);
        }

    }
}
