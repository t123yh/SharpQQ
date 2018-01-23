using System;

namespace SharpQQ.Binarizer.Structured
{
    public abstract class PacketFieldAttribute : Attribute
    {
        public int Index { get; }

        protected PacketFieldAttribute(int index)
        {
            this.Index = index;
        }

        public abstract void WriteValue(object val, BinaryBufferWriter writer);

        public abstract object ReadValue(Type target, BinaryBufferReader reader);
    }
}