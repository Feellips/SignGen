namespace SignGen.Models
{
    public interface IByteBlock
    {
        int Id { get; }
        byte[] Data { get; }
    }
}
