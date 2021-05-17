using System;
using System.Collections.Generic;
using System.Text;

namespace SignGen.Library
{
    public class ByteBlock
    {
        public int ID { get; }
        public byte[] Block { get; }

        public ByteBlock(int id, byte[] block)
        {
            ID = id;
            Block = block;
        }
        public static ByteBlock GetByteBlock(int id, byte[] block) => new ByteBlock(id, block);

        public static byte[] ByteBlockToByteArray(string byteBlock)
        {
            return Encoding.ASCII.GetBytes(byteBlock);
        } 
    }
}
