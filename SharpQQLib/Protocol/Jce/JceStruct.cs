using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace SharpQQ.Protocol.Jce
{
    // The following code is a piece of shit.
    // Don't look at it, just use it.

    public abstract class JceStruct
    {
        protected JceStruct()
        {
        }

        protected IEnumerable<PropertyInfo> DataProperties
        {
            get
            {
                return this.GetType()
                    .GetProperties()
                    .Where(x => Attribute.IsDefined(x, typeof(JceFieldAttribute), true));
            }
        }


        public void ReadFrom(JceInputStream stream)
        {
            foreach (var prop in DataProperties)
            {
                var attr = prop.GetCustomAttribute<JceFieldAttribute>();
                int tag = attr.Tag;
                var type = prop.PropertyType;
                if (Nullable.GetUnderlyingType(type) != null)
                {
                    type = Nullable.GetUnderlyingType(type);
                }
                prop.SetValue(this, stream.Read(type, prop.GetValue(this), tag, attr.Required));
            }
        }

        public void WriteTo(JceOutputStream stream)
        {
            foreach (var prop in DataProperties)
            {
                var attr = prop.GetCustomAttribute<JceFieldAttribute>();
                if (prop.GetValue(this) != null)
                {
                    stream.Write(prop.GetValue(this), attr.Tag);
                }
                else if (attr.Required)
                {
                    throw new JceEncodeException($"Field {prop.Name} is required but not presented.");
                }
            }
        }
    }
}