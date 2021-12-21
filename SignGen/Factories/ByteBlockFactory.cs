using SignGen.MultithreadExecution.Factories;
using SignGen.MultithreadExecution.Models;

namespace SignGen.Factories
{
    public class ByteBlockFactory : IByteBlockFactory
    {
        public IByteBlock Create(int id, byte[] data) => new ByteBlock(id, data);
    }
}