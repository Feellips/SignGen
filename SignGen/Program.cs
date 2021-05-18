using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SignGen.Exstensions;
using SignGen.Library;

namespace SignGen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                SignatureGeneratorStarter.Start(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
