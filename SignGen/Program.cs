using System;
using System.Diagnostics;
using System.IO;
using SignGen.Library;

namespace SignGen
{
    class Program
    {
        static void Main(string[] args)
        {
            args.Rules().Validate(); // todo

            var path = @"C:\Users\Ffedyushin\Downloads\ALT BİLEZİK.SLDPRT";
            var blockSize = 1024;

            using var input = File.Open(path, FileMode.Open, FileAccess.Read);
            using var sMem = new MemoryStream();

            using var signGen = new SignatureGenerator(input, sMem, blockSize);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            signGen.Start();
            
            Console.WriteLine(stopWatch.Elapsed);

        }
    }
}
