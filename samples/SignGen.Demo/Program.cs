using System;
using SignGen.Demo.Exstensions;

namespace SignGen.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new string[] { @"C:\Users\filf\Downloads\ubuntu-20.04.3-desktop-amd64.iso", "1024" };

            try
            {
                args.Rules().
                    IsNotEmpty().
                    IsEnoughArgs(2).
                    IsSourceFileExist(args[0]).
                    IsBlockSizeValid(args[1]);

                var filename = args[0];
                var blockSize = int.Parse(args[1]);

                var fileSignature = new FileSignature(filename, blockSize);
                fileSignature.WriteFileSignatureInConsole();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
