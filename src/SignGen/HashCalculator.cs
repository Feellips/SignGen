using System.Security.Cryptography;
using System.Text;

namespace SignGen
{
    public class HashCalculator
    {
        public string Compute(byte[] byteBlock)
        {
            var hashAlgorithm = SHA256.Create();
            var data = hashAlgorithm.ComputeHash(byteBlock);
            var sBuilder = new StringBuilder();

            foreach (var item in data)
            {
                sBuilder.Append(item.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}