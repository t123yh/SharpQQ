using System;
using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Binarizer.Tlv
{
    public class TlvPacketAttribute : Attribute
    {
        public short Tag { get; set; }

        public TlvPacketAttribute(short tag)
        {
            this.Tag = tag;
        }
    }
}