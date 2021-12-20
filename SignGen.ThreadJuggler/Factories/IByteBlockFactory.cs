using SignGen.ThreadJuggler.Models;

namespace SignGen.ThreadJuggler.Factories
{
    public interface IByteBlockFactory
    {
        IByteBlock Create(int id, byte[] data);
    }
}
