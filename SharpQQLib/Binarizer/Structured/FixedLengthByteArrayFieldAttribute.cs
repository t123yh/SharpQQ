using System;
using System.Linq;

using CodingContext = System.Collections.ObjectModel.ReadOnlyDictionary<string, dynamic>;

namespace SharpQQ.Binarizer.Structured
{
    public class FixedLengthByteArrayFieldAttribute : PacketFieldAttribute
    {
        public int Length { get; set; }

        public FixedLengthByteArrayFieldAttribute(int index, int length) : base(index)
        {
            this.Length = length;
        }

        public override void WriteValue(object val, BinaryBufferWriter writer, CodingContext context)
        {
            if (!(val is byte[] ba))
            {
                throw new ArgumentException($"Parameter must be a byte array instead of {val.GetType().ToString()}.");
            }
            byte[] result = new byte[this.Length];
            Array.Copy(ba, result, Math.Min(this.Length, ba.Length));
            writer.WriteByteArray(result);
        }

        public override object ReadValue(Type target, BinaryBufferReader reader, CodingContext context)
        {
            if (target != typeof(byte[]))
                throw new ArgumentException($"Unable to convert {target.Name} to byte array.");
            return reader.ReadByteArray(this.Length).ToArray();
        }
    }
}