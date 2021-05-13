//using System;
//using System.IO;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading;
//using SignGen.Library;
//using Xunit;

//namespace SignGen.Tests
//{
//    public class BlockingQueueWrapperTest
//    {
        
//        #region Fields

//        CustomThreadPool bqw;

//        Random rand = new Random();

//        Stream input;
//        Stream output = new MemoryStream();

//        #endregion

//        #region TestSequences

//        private Stream GetTestSequence(Func<int, int> converter)
//        {
//            var ms = new MemoryStream();

//            for (int i = 0; i < 10_000_000; i++)
//            {
//                byte[] intBytes = BitConverter.GetBytes(converter.Invoke(i));

//                ms.Write(intBytes);
//            }
//            ms.Position = 0;

//            return ms;
//        }

//        #region Converters
//        private int ConverterSimple(int num) => num;
//        private int ConverterRandom(int num) => rand.Next();
//        private int ConverterIncrement(int num) => ++num;
//        #endregion

//        #endregion

//        private void TestAction(ByteBlock byteBlock)
//        {
//            Thread.Sleep(rand.Next(1, 500));

//            rand.NextBytes(byteBlock.Block);

//            byteBlock.Compressed = new MemoryStream(byteBlock.Block);
//        }

//        private void TestAction2(ByteBlock byteBlock)
//        {
//            Thread.Sleep(rand.Next(1, 500));
//            byteBlock.Compressed = new MemoryStream(byteBlock.Block);
//        }


//        [Fact]
//        public void SimpleTest()
//        {
//            Assert.True(StreamsAreEqual(ConverterSimple, TestAction2));
//        }

//        [Fact]
//        public void IncrementTest()
//        {
//            Assert.True(StreamsAreEqual(ConverterIncrement, TestAction2));
//        }

//        [Fact]
//        public void RandomTest()
//        {
//            Assert.True(StreamsAreEqual(ConverterRandom, TestAction2));
//        }

//        [Fact]
//        public void RandomTest2()
//        {
//            Assert.True(StreamsAreEqual(ConverterRandom, TestAction2));
//        }

//        [Fact]
//        public void RandomTest3()
//        {
//            Assert.True(StreamsAreEqual(ConverterRandom, TestAction2));
//        }

//        [Fact]
//        public void RandomTest4()
//        {
//            Assert.True(StreamsAreEqual(ConverterRandom, TestAction2));
//        }

//        [Fact]
//        public void ConversionTest()
//        {
//            Assert.False(StreamsAreEqual(ConverterRandom, TestAction));
//        }

//        private bool StreamsAreEqual(Func<int, int> converter, Action<ByteBlock> action)
//        {

//            using (var input = GetTestSequence(converter))
//            using (var md5 = MD5.Create())
//            {
//                var bqw = new CustomThreadPool(action, input, output, 8);
//                bqw.Begin();
//                bqw.Dispose();

//                input.Position = 0;
//                output.Position = 0;

//                var inHash = GetHash(md5, input);
//                var outHash = GetHash(md5, output);

//                return inHash == outHash;
//            }

//        }

//        private static string GetHash(HashAlgorithm hashAlgorithm, Stream stream)
//        {

//            byte[] data = hashAlgorithm.ComputeHash(stream);

//            var sBuilder = new StringBuilder();

//            for (int i = 0; i < data.Length; i++)
//            {
//                sBuilder.Append(data[i].ToString("x2"));
//            }

//            return sBuilder.ToString();
//        }
//    }
//}