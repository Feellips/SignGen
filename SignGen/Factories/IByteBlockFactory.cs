using SignGen.Models;

namespace SignGen.Factories
{
    public interface IByteBlockFactory
    {
        IByteBlock Create(int id, byte[] data);
    }
}
