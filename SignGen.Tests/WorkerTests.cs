using System;
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


        private bool SimpleTest(string str)
        {
            var worker = new Worker<string, string>(emptyFunc);

            worker.FeedData(str);

            return str == worker.Result;
        }

        private bool ReusabilityTest(string str, string str2)
        {
            var worker = new Worker<string, string>(emptyFunc);

            worker.FeedData(str);

            bool first = str == worker.Result;

            worker.FeedData(str2);

            bool second = str2 == worker.Result;

            return first && second;
        }

    }
}
