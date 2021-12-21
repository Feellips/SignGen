using SignGen.MultithreadExecution.Models;

namespace SignGen.MultithreadExecution.Factories
{
    public interface IByteBlockFactory
    {
        IByteBlock Create(int id, byte[] data);
    }
}
