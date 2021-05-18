using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SignGen.Exstensions;
using SignGen.Library;

namespace SignGen
{
    public class SignatureGeneratorStarter
    {
        public static void Start(string[] args)
        {
            args.Rules().
                IsNotNull().
                IsEnoughArgs(2).
                IsPathCorrect(args[0]).
                IsSourceFileExist().
                IsBlockSizeValid();


            var path = args[0];
            var blockSize = int.Parse(args[1]);
            var threadCount = Environment.ProcessorCount;

            var input = File.Open(path, FileMode.Open, FileAccess.Read);
            var output = Console.OpenStandardOutput();

            using var signGen = new SignatureGenerator(input, output, blockSize);

            signGen.Start(threadCount);
        }
    }
}
