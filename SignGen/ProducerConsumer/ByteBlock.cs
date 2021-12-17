using System.Text;

namespace SignGen.ProducerConsumer
{
    public class ByteBlock
    {
        public int Id { get; }
        public byte[] Block { get; }

        private ByteBlock(int id, byte[] block)
        {
            Id = id;
            Block = block;
        }
        public static ByteBlock GetByteBlock(int id, byte[] block) => new ByteBlock(id, block);
        public static byte[] ToByteArray(string byteBlock) => Encoding.ASCII.GetBytes(byteBlock);
    }
}
