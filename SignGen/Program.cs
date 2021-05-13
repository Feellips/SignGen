using System;
using System.IO;
using SignGen.Library;

namespace SignGen
{
    class Program
    {
        static void Main(string[] args)
        {
            args.Rules().Validate(); // todo

            var path = @"C:\Users\PhedyushinPhilipp\Downloads\Cyberpunk 2077 RePack by SE7EN\data_04.bin";
            var blockSize = 1024;

            //using var input = new StreamReader(path);
           // using var input = new StreamReader(@"C:\Users\PhedyushinPhilipp\Desktop\test.txt");

            //using var signGen = new MultithreadedSignatureGenerator(input, Console.Out, blockSize);

//            signGen.Start();

        }
    }
}
