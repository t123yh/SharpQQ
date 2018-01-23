using System;
using System.Linq;

namespace SharpQQ.Binarizer.Structured
{
    public class FixedLengthByteArrayFieldAttribute : PacketFieldAttribute
    {
        public int Length { get; }

        public FixedLengthByteArrayFieldAttribute(int index, int length) : base(index)
        {
            this.Length = length;
        }

        public override void WriteValue(object val, BinaryBufferWriter writer)
        {
            if (!(val is byte[] ba))
            {
                throw new ArgumentException($"Parameter must be a byte array instead of {val.GetType().FullName}.");
            }
            byte[] result = new byte[this.Length];
            Array.Copy(ba, result, Math.Min(this.Length, ba.Length));
            writer.WriteByteArray(result);
        }

        public override object ReadValue(Type target, BinaryBufferReader reader)
        {
            if (target != typeof(byte[]))
                throw new ArgumentException($"Unable to convert {target.FullName} to byte array.");
            return reader.ReadByteArray(this.Length).ToArray();
        }
    }
}