using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SignGen.Library;

namespace SignGen
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                args.Rules().
                    IsNotNull().
                    IsEnoughArgs(2).
                    IsPathCorrect(args[0]).
                    IsSourceFileExist().
                    IsBlockSizeValid();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var path = args[0];
            var blockSize = int.Parse(args[1]);

            using var input = File.Open(path, FileMode.Open, FileAccess.Read);
            using var output = Console.OpenStandardOutput();

            using var signGen = new SignatureGenerator(input, output, blockSize);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            signGen.Start();
            
            Console.WriteLine(stopWatch.Elapsed);

        }
    }
}
