using System;
using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Binarizer.Tlv
{
    public class TlvPacketContent : Attribute
    {
        public short Tag { get; set; }

        public TlvPacketContent(short tag)
        {
            this.Tag = tag;
        }
    }
}