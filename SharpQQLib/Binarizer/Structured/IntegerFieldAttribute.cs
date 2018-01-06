using System;
using System.Linq;
using SharpQQ.Utils;

namespace SharpQQ.Binarizer.Structured
{
    public class IntegerFieldAttribute : PacketFieldAttribute
    {
        public Endianness Endianness { get; }

        public bool Optional { get; }

        public IntegerFieldAttribute(int index, Endianness endianness = Endianness.Big, bool optional = false) : base(index)
        {
            this.Endianness = endianness;
            this.Optional = optional;
        }

        public override void WriteValue(object val, BinaryBufferWriter writer)
        {
            if (val != null)
            {
                Type valType = val.GetType();
                object numVal;
                if (valType.IsEnum)
                {
                    Type underlying = valType.GetEnumUnderlyingType();
                    numVal = Convert.ChangeType(val, underlying);
                }
                else
                {
                    numVal = val;
                }
                writer.WriteByteArray(MyBitConverter.IntegerToBytes(numVal, this.Endianness));
            }
            else
            {
                if (!this.Optional)
                    throw new NullReferenceException("This field must not be null.");
            }
        }

        public override object ReadValue(Type targetType, BinaryBufferReader reader)
        {
            if (MyBitConverter.IntegerSizes.ContainsKey(targetType))
            {
                if (!reader.IsEndOfStream)
                {
                    if (targetType.IsEnum)
                    {
                        Type numericType = targetType.GetEnumUnderlyingType();
                        byte[] data = reader.ReadByteArray(MyBitConverter.IntegerSizes[numericType]).ToArray();
                        object value = MyBitConverter.BytesToInteger(data, numericType, this.Endianness);
                        return Enum.ToObject(targetType, value);
                    }
                    else
                    {
                        byte[] data = reader.ReadByteArray(MyBitConverter.IntegerSizes[targetType]).ToArray();
                        return MyBitConverter.BytesToInteger(data, targetType, this.Endianness);
                    }
                }
                else
                {
                    if (!this.Optional)
                        throw new ArgumentException("Unable to read from stream.");
                    else
                        return null;
                }
            }
            else
            {
                throw new ArgumentException($"Target must be one of the integer types instead of {targetType.FullName}");
            }
        }
    }
}