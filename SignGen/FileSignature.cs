using System;
using System.IO;
using SignGen.Library;

namespace SignGen
{
    public class FileSignature
    {
        private readonly string _path;

        public FileSignature(string path)
        {
            _path = path;
        }
        public void WriteFileSignatureInConsole(int blockSize) =>
            WriteFileSignatureInConsole(blockSize, Environment.ProcessorCount);
        
        public void WriteFileSignatureInConsole(int blockSize, int threads)
        {
            using var input = File.Open(_path, FileMode.Open, FileAccess.Read);
            using var output = Console.OpenStandardOutput();
            using var signGen = new SignatureGenerator(input, output, blockSize);

            signGen.Start(threads);
        }     
    }
}
