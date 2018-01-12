using System;
using SharpQQ.Utils;
using SharpQQ.Binarizer;
using System.Linq;

namespace SharpQQ.Binarizer.Structured
{
    public class VariableLengthSubstructFieldAttribute : VariableLengthFieldAttribute
    {
        public VariableLengthSubstructFieldAttribute(int index, PrependLengthType type, PrependLengthTransform transform = PrependLengthTransform.NoTransform) : base(index, type, transform)
        {

        }

        protected override byte[] ConvertToByteArray(object val)
        {
            if (!(val is IBinaryConvertible ba))
            {
                throw new ArgumentException($"Parameter must be an IBinaryConvertible");
            }
            var writer = new BinaryBufferWriter();
            ba.WriteTo(writer);
            return writer.GetContent();
        }

        protected override object ParseFromByteArray(Type targetType, byte[] buf)
        {
            if (!typeof(IBinaryConvertible).IsAssignableFrom(targetType))
                throw new ArgumentException($"Type {targetType.Name} does not implement IBinaryConvertible.");
            var val = (IBinaryConvertible)Activator.CreateInstance(targetType);
            val.ParseFrom(new BinaryBufferReader(buf));
            return val;
        }
    }
}