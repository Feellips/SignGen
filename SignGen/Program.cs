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

            var path = @"C:\ProgramData\Autodesk\Inventor 2021\Content Center\Libraries\AI2021_Inventor OTHER.idcl";
            var blockSize = 4096;

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
