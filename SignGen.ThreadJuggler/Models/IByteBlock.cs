namespace SignGen.MultithreadExecution.Models
{
    public interface IByteBlock
    {
        int Id { get; }
        byte[] Data { get; }
    }
}
