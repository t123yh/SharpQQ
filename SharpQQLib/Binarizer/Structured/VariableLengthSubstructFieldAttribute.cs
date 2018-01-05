using System;
using SharpQQ.Utils;
using SharpQQ.Binarizer;
using System.Linq;

using CodingContext = System.Collections.ObjectModel.ReadOnlyDictionary<string, dynamic>;

namespace SharpQQ.Binarizer.Structured
{
    public class VariableLengthSubstructFieldAttribute : VariableLengthFieldAttribute
    {
        public VariableLengthSubstructFieldAttribute(int index, PrependLengthType type, PrependLengthTransform transform = PrependLengthTransform.NoTransform) : base(index, type, transform)
        {

        }

        protected override byte[] ConvertToByteArray(object val, CodingContext context)
        {
            if (!(val is BinaryPacket ba))
            {
                throw new ArgumentException($"Parameter must be an IBinaryConvertible");
            }
            var writer = new BinaryBufferWriter();
            ba.WriteTo(writer, context);
            return writer.GetContent();
        }

        protected override object ParseFromByteArray(Type targetType, byte[] buf, CodingContext context)
        {
            if (!typeof(BinaryPacket).IsAssignableFrom(targetType))
                throw new ArgumentException($"Type {targetType.Name} does not implement IBinaryConvertible.");
            var val = (BinaryPacket)Activator.CreateInstance(targetType);
            val.ParseFrom(new BinaryBufferReader(buf), context);
            return val;
        }
    }
}