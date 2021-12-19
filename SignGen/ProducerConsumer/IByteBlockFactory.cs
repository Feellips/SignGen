namespace SignGen.ProducerConsumer
{
    public interface IByteBlockFactory
    {
        IByteBlock Create(int id, byte[] data);
    }
}
