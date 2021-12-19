namespace SignGen.ProducerConsumer
{
    public class ByteBlockFactory : IByteBlockFactory
    {
        public IByteBlock Create(int id, byte[] data) => new ByteBlock(id, data);
    }
}