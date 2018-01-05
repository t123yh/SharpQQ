using System;

using CodingContext = System.Collections.ObjectModel.ReadOnlyDictionary<string, dynamic>;

namespace SharpQQ.Binarizer.Structured
{
    public abstract class PacketFieldAttribute : Attribute
    {
        public int Index { get; }

        public PacketFieldAttribute(int index)
        {
            this.Index = index;
        }

        public abstract void WriteValue(object val, BinaryBufferWriter writer, CodingContext context);

        public abstract object ReadValue(Type target, BinaryBufferReader reader, CodingContext context);
    }
}