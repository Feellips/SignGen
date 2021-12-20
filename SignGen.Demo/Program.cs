﻿using System;
using SignGen.Demo.Exstensions;

namespace SignGen.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
           args = new[] { @"D:\Program Files (x86)\Rocksmith 2014\audio.psarc", "1024"};
            
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
