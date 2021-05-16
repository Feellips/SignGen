using System;
using System.Collections.Generic;
using System.Text;

namespace SignGen.Library
{
    public class HashBlock
    {
        public int ID { get; }
        public byte[] ByteBlock { get; }

        public HashBlock(int id, byte[] byteBlock)
        {
            ID = id;
            ByteBlock = byteBlock;
        }
    }
}
