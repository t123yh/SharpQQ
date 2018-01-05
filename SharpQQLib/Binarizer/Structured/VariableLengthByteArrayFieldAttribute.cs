using System;
using System.Text;

using CodingContext = System.Collections.ObjectModel.ReadOnlyDictionary<string, dynamic>;

namespace SharpQQ.Binarizer.Structured
{
    public class VariableLengthByteArrayFieldAttribute : VariableLengthFieldAttribute
    {
        public static readonly Encoding DefaultTextEncoding = Encoding.UTF8;

        public VariableLengthByteArrayFieldAttribute(int index, PrependLengthType type, PrependLengthTransform transform = PrependLengthTransform.NoTransform) : base(index, type, transform)
        {
        }

        protected override byte[] ConvertToByteArray(object val, CodingContext context)
        {
            if (val is string str)
                return DefaultTextEncoding.GetBytes(str);
            else if (val is byte[] ba)
                return ba;
            throw new ArgumentException($"Parameter must be a byte array instead of {val.GetType().ToString()}.");
        }

        protected override object ParseFromByteArray(Type targetType, byte[] buf, CodingContext context)
        {
            if (targetType == typeof(string))
                return DefaultTextEncoding.GetString(buf);
            else if (targetType == typeof(byte[]))
                return buf;
            throw new ArgumentException($"Unable to convert {targetType.Name} to byte array.");
        }
    }
}