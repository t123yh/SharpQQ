using System;
using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Binarizer.Tlv
{
    public class TlvPacketContentAttribute : Attribute
    {
        public short Tag { get; set; }

        public TlvPacketContentAttribute(short tag)
        {
            this.Tag = tag;
        }
    }
}