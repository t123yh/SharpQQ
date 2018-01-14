using System;
using System.Linq;
using System.Text;

namespace SharpQQ.Binarizer.Structured
{
    public class VariableLengthByteArrayFieldAttribute : VariableLengthFieldAttribute
    {
        public int MaxLength { get; set; }
        public static readonly Encoding DefaultTextEncoding = Encoding.UTF8;

        public VariableLengthByteArrayFieldAttribute(int index, PrependLengthType type,
            PrependLengthTransform transform = PrependLengthTransform.NoTransform, int maxLength = -1) : base(index,
            type, transform)
        {
            this.MaxLength = maxLength;
        }

        private bool CheckLength(int len)
        {
            return (MaxLength < 0 || len <= MaxLength);
        }

        protected override byte[] ConvertToByteArray(object val)
        {
            byte[] result;
            if (val is string str)
                result = DefaultTextEncoding.GetBytes(str);
            else if (val is byte[] ba)
                result = ba;
            else
                throw new ArgumentException($"Parameter must be a byte array instead of {val.GetType().ToString()}.");

            return CheckLength(result.Length) ? result : result.Take(MaxLength).ToArray();
        }

        protected override object ParseFromByteArray(Type targetType, byte[] buf)
        {
            if (!CheckLength(buf.Length))
                buf = buf.Take(MaxLength).ToArray();

            if (targetType == typeof(string))
                return DefaultTextEncoding.GetString(buf);
            else if (targetType == typeof(byte[]))
                return buf;
            throw new ArgumentException($"Unable to convert {targetType.Name} to byte array.");
        }
    }
}