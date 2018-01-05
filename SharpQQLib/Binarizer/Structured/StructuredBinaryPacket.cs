using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodingContext = System.Collections.ObjectModel.ReadOnlyDictionary<string, dynamic>;

namespace SharpQQ.Binarizer.Structured
{
    public abstract class StructuredBinaryPacket : BinaryPacket
    {
        public virtual void Check() { }

        public override void ParseFrom(BinaryBufferReader reader, CodingContext context)
        {
            foreach(var prop in this.DataProperties)
            {
                var attr = prop.GetCustomAttribute<PacketFieldAttribute>();
                object val = attr.ReadValue(prop.PropertyType, reader, context);
                prop.SetValue(this, val);
            }
            Check();
        }

        public override void WriteTo(BinaryBufferWriter writer, CodingContext context)
        {
            Check();
            foreach (var prop in this.DataProperties)
            {
                Console.WriteLine("Writing to " + prop.Name);
                var attr = prop.GetCustomAttribute<PacketFieldAttribute>();
                object val = prop.GetValue(this);
                attr.WriteValue(val, writer, context);
            }
        }

        protected IEnumerable<PropertyInfo> DataProperties
        {
            get
            {
                return this.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => Attribute.IsDefined(x, typeof(PacketFieldAttribute), true))
                    .OrderBy(x => x.GetCustomAttribute<PacketFieldAttribute>().Index);
            }
        }
    }
}