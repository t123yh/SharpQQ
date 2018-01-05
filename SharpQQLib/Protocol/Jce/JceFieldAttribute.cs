using System;
using System.Reflection;
using System.IO;
using System.Linq;

namespace SharpQQ.Protocol.Jce
{


    [AttributeUsage(AttributeTargets.Property)]
    public class JceFieldAttribute : Attribute
    {
        public JceFieldAttribute(byte tag, bool required)
        {
            this.Tag = tag;
            this.Required = required;
        }

        public byte Tag { get; set; }

        public bool Required { get; set; }
    }
}