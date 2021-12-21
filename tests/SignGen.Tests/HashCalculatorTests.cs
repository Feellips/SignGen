using System.Text;
using Xunit;

namespace SignGen.Tests
{
    public class HashCalculatorTests
    {
        private const string SimpleString = "Hello, world!";

        // string from sha256 online encoding service
        private const string ExpectedHash = "315F5BDB76D078C43B8AC0064E4A0164612B1FCE77C869345BFC94C75894EDD3";

        [Fact]
        private void HashCalculator_PassSimpleString_ValidHash()
        {
            var hashCalculator = new HashCalculator();
            var hash = hashCalculator.Compute(Encoding.UTF8.GetBytes(SimpleString));

            Assert.Equal(ExpectedHash, hash, ignoreCase: true);
        }

    }
}
