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
        public ArgsValidator IsSourceFileExist()
        {
            if (!File.Exists(args[0]))
                throw new ArgumentException("Error: Source file does not exist.");

            return this;
        }
        public ArgsValidator IsPathCorrect(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Error: Path can not be empty.");

            string file = Path.GetFileName(path);
            string dir = Path.GetDirectoryName(path);

            bool fileInvalid = file.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
            bool dirInvalid = dir.IndexOfAny(Path.GetInvalidPathChars()) >= 0;

            if (fileInvalid || dirInvalid)
                throw new ArgumentException($"Error: {path} does not valid.");

            return this;
        }

        public ArgsValidator IsBlockSizeValid()
        {
            if (!int.TryParse(args[1], out int result))
                throw new ArgumentException($"{args[1]} is not a number.");

            if (result < 0) throw new ArgumentException("Error: block size must be positive.");
            if (result == 0) throw new ArgumentException("Error: block size must be more than zero.");

            return this;
        }
    }
}