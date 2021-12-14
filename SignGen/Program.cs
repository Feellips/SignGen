using System;
using SignGen.Exstensions;

namespace SignGen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                args.Rules().
                    IsNotNull().
                    IsEnoughArgs(2).
                    IsSourceFileExist(0).
                    IsBlockSizeValid(1);

                var path = args[0];
                var blockSize = int.Parse(args[1]);

                var fileSignature = new FileSignature(path);
                fileSignature.WriteFileSignatureInConsole(blockSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
