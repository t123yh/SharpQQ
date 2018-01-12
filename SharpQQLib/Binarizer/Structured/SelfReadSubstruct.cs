using System;
using System.Linq;

namespace SharpQQ.Binarizer.Structured
{
    public class SelfReadSubstructFieldAttribute : PacketFieldAttribute
    {
        public SelfReadSubstructFieldAttribute(int index) : base(index)
        {

        }

        public override void WriteValue(object val, BinaryBufferWriter writer)
        {
            if (!(val is IBinaryConvertible ba))
            {
                throw new ArgumentException($"Parameter must be an IBinaryConvertible");
            }
            ba.WriteTo(writer);
        }

        public override object ReadValue(Type targetType, BinaryBufferReader reader)
        {
            if (!targetType.GetInterfaces().Contains(typeof(IBinaryConvertible)))
                throw new ArgumentException($"Type {targetType.Name} does not implement IBinaryConvertible.");
            var val = (IBinaryConvertible)Activator.CreateInstance(targetType);
            val.ParseFrom(reader);
            return val;
        }
    }
}