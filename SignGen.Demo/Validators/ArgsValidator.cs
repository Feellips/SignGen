using System;
using System.IO;
using System.Linq;

namespace SignGen.Demo.Validators
{
    public class ArgsValidator
    {
        private string[] args;

        public ArgsValidator(string[] args) => this.args = args;

        public ArgsValidator IsNotEmpty()
        {
            if (args.Any() == false)
                throw new ArgumentException(
                    "Please specify filename and block size in such manner: SignGen.exe [filename] [block size]");

            return this;
        }

        public ArgsValidator IsEnoughArgs(int num)
        {
            if (args.Length != num)
                throw new ArgumentException($"Error: Wrong number of arguments. Supposed to be {num}");

            return this;
        }

        public ArgsValidator IsSourceFileExist(string filename)
        {
            if (File.Exists(filename) == false)
                throw new ArgumentException("Error: Source file does not exist.");

            return this;
        }

        public ArgsValidator IsBlockSizeValid(string blockSize)
        {
            if (int.TryParse(blockSize, out int result) == false)
                throw new ArgumentException($"{blockSize} is not a number.");

            if (result <= 0) throw new ArgumentException("Error: block size must be positive and more than zero.");

            return this;
        }
    }
}