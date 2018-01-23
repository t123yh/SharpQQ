using System;
using System.Linq;
using SharpQQ.Utils;

namespace SharpQQ.Binarizer.Structured
{
    public class IntegerFieldAttribute : PacketFieldAttribute
    {
        public Endianness Endianness { get; }

        public bool Optional { get; }

        public IntegerFieldAttribute(int index, Endianness endianness = Endianness.Big, bool optional = false) :
            base(index)
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
            bool isEnum = targetType.IsEnum;
            var numericType = isEnum ? targetType.GetEnumUnderlyingType() : targetType;

            if (MyBitConverter.IntegerSizes.ContainsKey(numericType))
            {
                if (!reader.IsEndOfStream)
                {
                    byte[] data = reader.ReadByteArray(MyBitConverter.IntegerSizes[numericType]).ToArray();
                    if (isEnum)
                    {
                        var value = MyBitConverter.BytesToInteger(data, numericType, this.Endianness);
                        return Enum.ToObject(targetType, value);
                    }
                    else
                    {
                        return MyBitConverter.BytesToInteger(data, targetType, this.Endianness);
                    }
                }
                else
                {
                    if (!this.Optional)
                        throw new BinarizerException("Unable to read from stream for a non-optional field.");
                    else
                        return null;
                }
            }
            else
            {
                throw new ArgumentException(
                    $"Target must be one of the integer types instead of {targetType.FullName}");
            }
        }
    }
}