namespace SignGen.ProducerConsumer
{
    public interface IByteBlock
    {
        int Id { get; }
        byte[] Data { get; }
    }
}
