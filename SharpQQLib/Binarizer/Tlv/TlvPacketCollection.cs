using System;
using System.Collections;
using SharpQQ.Binarizer;
using SharpQQ.Binarizer.Structured;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SharpQQ.Utils;

namespace SharpQQ.Binarizer.Tlv
{
    public class TlvConvertibleCollection : Dictionary<short, byte[]>, IBinaryConvertible
    {
        public short Tag { get; set; } = -1;

        public TlvConvertibleCollection()
        {
        }

        public TlvConvertibleCollection(short tag)
        {
            this.Tag = tag;
        }

        public void Add(IBinaryConvertible convertible)
        {
            short tag = convertible.GetType().GetCustomAttribute<TlvPacketContent>().Tag;
            base.Add(tag, convertible.GetBinary());
        }

        public T Get<T>() where T : IBinaryConvertible
        {
            short tag = typeof(T).GetCustomAttribute<TlvPacketContent>().Tag;
            var val = base[tag];
            var result = Activator.CreateInstance<T>();
            result.ParseFrom(val);
            return result;
        }

        public void ParseFrom(BinaryBufferReader reader)
        {
            base.Clear();
            
            short tag = reader.ReadInt16(Endianness.Big);
            this.Tag = tag;
            
            short fieldCount = reader.ReadInt16(Endianness.Big);
            for (int i = 0; i < fieldCount; i++)
            {
                short subTag = reader.ReadInt16(Endianness.Big);
                short subLength = reader.ReadInt16(Endianness.Big);
                byte[] dat = reader.ReadByteArray(subLength).ToArray();
                base.Add(subTag, dat);
            }
        }

        public void WriteTo(BinaryBufferWriter writer)
        {
            if (this.Tag == -1)
                throw new Exception("You should set a Tag for TlvPacketCollection before encoding it.");
            
            writer.WriteInt16(this.Tag, Endianness.Big);
            writer.WriteInt16((short)base.Count, Endianness.Big);
            foreach (var item in this)
            {
                writer.WriteInt16(item.Key, Endianness.Big); // Tag
                writer.WriteInt16((short)item.Value.Length, Endianness.Big);
                writer.WriteByteArray(item.Value);
            }
        }
    }
}