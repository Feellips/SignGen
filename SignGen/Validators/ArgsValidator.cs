using System;
using System.IO;

namespace SignGen.Validators
{
    public class ArgsValidator
    {
        private string[] args;

        public ArgsValidator(string[] args) => this.args = args;

        public ArgsValidator IsNotNull()
        {
            if (args == null)
                throw new ArgumentNullException("Error: No arguments were found.");

            return this;
        }
        public ArgsValidator IsEnoughArgs(int num)
        {
            if (args.Length != num)
                throw new ArgumentException($"Error: Wrong number of arguments. Supposed to be {num}");

            return this;
        }
        public ArgsValidator IsSourceFileExist(int argPosition)
        {
            if (!File.Exists(args[argPosition]))
                throw new ArgumentException("Error: Source file does not exist.");

            return this;
        }

        public ArgsValidator IsBlockSizeValid(int argPosition)
        {
            if (!int.TryParse(args[argPosition], out int result))
                throw new ArgumentException($"{args[argPosition]} is not a number.");

            if (result <= 0) throw new ArgumentException("Error: block size must be positive and more than zero.");

            return this;
        }
    }
}