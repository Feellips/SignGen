using SignGen.MultithreadExecution.Models;

namespace SignGen.Models
{
    public class ByteBlock : IByteBlock
    {
        public int Id { get; }
        public byte[] Data { get; }

        public ByteBlock(int id, byte[] data)
        {
            Id = id;
            Data = data;
        }
    }
}
