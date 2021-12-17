using System;
using System.IO;

namespace SignGen.Demo
{
    public class FileSignature
    {
        private readonly string _path;
        private readonly int _blockSize;

        public FileSignature(string path, int blockSize)
        {
            _path = path;
            _blockSize = blockSize;
        }

        public void WriteFileSignatureInConsole() => WriteFileSignatureInConsole(Environment.ProcessorCount);
        public void WriteFileSignatureInConsole(int threads)
        {
            using var input = File.Open(_path, FileMode.Open, FileAccess.Read);
            using var output = Console.OpenStandardOutput();
            using var signGen = new MultithreadedSignatureGenerator(input, output, _blockSize);

            signGen.Start(threads);
        }
    }
}
