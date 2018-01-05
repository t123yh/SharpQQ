using System;
using SharpQQ.Utils;
using System.Linq;

using CodingContext = System.Collections.ObjectModel.ReadOnlyDictionary<string, dynamic>;

namespace SharpQQ.Binarizer.Structured
{
    public enum PrependLengthTransform
    {
        NoTransform,

        IncludeLengthField,
    }

    public enum PrependLengthType
    {
        None,
        Int8,
        Int16BE,
        Int16LE,
        Int32BE,
        Int32LE
    }

    public abstract class VariableLengthFieldAttribute : PacketFieldAttribute
    {
        public PrependLengthType PrependType { get; }

        public PrependLengthTransform Transform { get; }

        public VariableLengthFieldAttribute(int index, PrependLengthType type, PrependLengthTransform transform) : base(index)
        {
            this.PrependType = type;
            this.Transform = transform;
        }

        private byte[] GetLength(int length)
        {
            if (this.PrependType == PrependLengthType.None)
                return new byte[0];
            else
            {
                int lengthDelta = 0, maxLength = 0;
                switch (this.PrependType)
                {
                    case PrependLengthType.Int8:
                        lengthDelta = 1;
                        maxLength = byte.MaxValue;
                        break;
                    case PrependLengthType.Int16LE:
                    case PrependLengthType.Int16BE:
                        lengthDelta = 2;
                        maxLength = short.MaxValue;
                        break;
                    case PrependLengthType.Int32LE:
                    case PrependLengthType.Int32BE:
                        lengthDelta = 4;
                        maxLength = int.MaxValue;
                        break;
                }
                int actualLength = length + (this.Transform == PrependLengthTransform.IncludeLengthField ? lengthDelta : 0);
                if (actualLength > maxLength)
                {
                    throw new ArgumentException($"Binary content too long: {maxLength} maximum, {actualLength} required.");
                }
                switch (this.PrependType)
                {
                    case PrependLengthType.Int8:
                        return new byte[1] { (byte)actualLength };
                    case PrependLengthType.Int16LE:
                        return MyBitConverter.GetBytesFromInt16((short)actualLength, Endianness.Little);
                    case PrependLengthType.Int16BE:
                        return MyBitConverter.GetBytesFromInt16((short)actualLength, Endianness.Big);
                    case PrependLengthType.Int32LE:
                        return MyBitConverter.GetBytesFromInt32((int)actualLength, Endianness.Little);
                    case PrependLengthType.Int32BE:
                        return MyBitConverter.GetBytesFromInt32((int)actualLength, Endianness.Big);
                    default:
                        throw new ArgumentException("Invalid PrependLengthType.");
                }
            }
        }

        private int ReadLength(BinaryBufferReader reader)
        {
            if (this.PrependType == PrependLengthType.None)
            {
                return reader.RemainingLength;
            }
            else
            {
                int delta, len;
                switch (this.PrependType)
                {
                    case PrependLengthType.Int8:
                        len = reader.ReadByte();
                        delta = 1;
                        break;
                    case PrependLengthType.Int16LE:
                        len = reader.ReadInt16(Endianness.Little);
                        delta = 2;
                        break;
                    case PrependLengthType.Int16BE:
                        len = reader.ReadInt16(Endianness.Big);
                        delta = 2;
                        break;
                    case PrependLengthType.Int32LE:
                        len = reader.ReadInt32(Endianness.Little);
                        delta = 4;
                        break;
                    case PrependLengthType.Int32BE:
                        len = reader.ReadInt32(Endianness.Big);
                        delta = 4;
                        break;
                    default:
                        throw new ArgumentException("Invalid PrependLengthType.");
                }
                if (this.Transform == PrependLengthTransform.IncludeLengthField)
                {
                    len -= delta;
                }
                return len;
            }
        }

        protected abstract byte[] ConvertToByteArray(object val, CodingContext context);

        protected abstract object ParseFromByteArray(Type type, byte[] buf, CodingContext context);

        public sealed override void WriteValue(object val, BinaryBufferWriter writer, CodingContext context)
        {
            byte[] dat = ConvertToByteArray(val, context);
            byte[] len = GetLength(dat.Length);
            writer.WriteByteArray(len);
            writer.WriteByteArray(dat);
        }

        public sealed override object ReadValue(Type targetType, BinaryBufferReader reader, CodingContext context)
        {
            int len = ReadLength(reader);
            byte[] dat = reader.ReadByteArray(len).ToArray();
            return ParseFromByteArray(targetType, dat, context);
        }
    }
}