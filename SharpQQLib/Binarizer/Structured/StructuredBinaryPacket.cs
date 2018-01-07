using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpQQ.Binarizer.Structured
{
    public abstract class StructuredBinaryPacket : BinaryPacket
    {
        public virtual void Check() { }

        public override void ParseFrom(BinaryBufferReader reader)
        {
            foreach(var prop in this.DataProperties)
            {
                var attr = prop.GetCustomAttribute<PacketFieldAttribute>();
                object val = attr.ReadValue(prop.PropertyType, reader);
                prop.SetValue(this, val);
            }
            Check();
        }

        public override void WriteTo(BinaryBufferWriter writer)
        {
            Check();
            foreach (var prop in this.DataProperties)
            {
                var attr = prop.GetCustomAttribute<PacketFieldAttribute>();
                object val = prop.GetValue(this);
                attr.WriteValue(val, writer);
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