using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SignGen.Exstensions;
using SignGen.Library;

namespace SignGen
{
    class Program
    {
        public static void Main(string[] args)
        {
            args = new string[2] { @"C:\Program Files\Pixologic\ZBrush 2019\ZProjects\character.ZPR", "4096" };

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
                return;
            }

            var path = args[0];
            var blockSize = int.Parse(args[1]);
            var threadCount = Environment.ProcessorCount;

            var input = File.Open(path, FileMode.Open, FileAccess.Read);
            var output = Console.OpenStandardOutput();

            try
            {
                using var signGen = new SignatureGenerator(input, output, blockSize);
                signGen.Start(threadCount);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
