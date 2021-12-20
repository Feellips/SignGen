using SignGen.ThreadJuggler.Factories;
using SignGen.ThreadJuggler.Models;

namespace SignGen.Factories
{
    public class ByteBlockFactory : IByteBlockFactory
    {
        public IByteBlock Create(int id, byte[] data) => new ByteBlock(id, data);
    }
}