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
    public class TlvConvertibleCollection : IBinaryConvertible, IDictionary<short, byte[]>
    {
        public short Tag { get; set; } = -1;
        public readonly Dictionary<short, byte[]> _fields = new Dictionary<short, byte[]>();

        public TlvConvertibleCollection()
        {
        }

        public TlvConvertibleCollection(short tag)
        {
            this.Tag = tag;
        }

        public void Add(IBinaryConvertible convertible)
        {
            short tag = convertible.GetType().GetCustomAttribute<TlvPacketAttribute>().Tag;
            _fields.Add(tag, convertible.GetBinary());
        }

        public void Add(short tag, byte[] dat)
        {
            _fields.Add(tag, dat);
        }

        public T Get<T>() where T : IBinaryConvertible
        {
            short tag = typeof(T).GetCustomAttribute<TlvPacketAttribute>().Tag;
            var val = _fields[tag];
            var result = Activator.CreateInstance<T>();
            result.ParseFrom(val);
            return result;
        }

        public void ParseFrom(BinaryBufferReader reader)
        {
            this._fields.Clear();
            
            short tag = reader.ReadInt16(Endianness.Big);
            this.Tag = tag;
            
            short fieldCount = reader.ReadInt16(Endianness.Big);
            for (int i = 0; i < fieldCount; i++)
            {
                short subTag = reader.ReadInt16(Endianness.Big);
                short subLength = reader.ReadInt16(Endianness.Big);
                byte[] dat = reader.ReadByteArray(subLength).ToArray();
                this._fields.Add(subTag, dat);
            }
        }

        public void WriteTo(BinaryBufferWriter writer)
        {
            writer.WriteInt16(this.Tag, Endianness.Big);
            writer.WriteInt16((short)this._fields.Count, Endianness.Big);
            foreach (var item in this._fields)
            {
                writer.WriteInt16(item.Key, Endianness.Big); // Tag
                writer.WriteInt16((short)item.Value.Length, Endianness.Big);
                writer.WriteByteArray(item.Value);
            }
        }
        
        public IEnumerator<KeyValuePair<short, byte[]>> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<short, byte[]> item)
        {
            _fields.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _fields.Clear();
        }

        public bool Contains(KeyValuePair<short, byte[]> item)
        {
            return ((IDictionary<short, byte[]>)_fields).Contains(item);
        }

        public void CopyTo(KeyValuePair<short, byte[]>[] array, int arrayIndex)
        {
            ((IDictionary<short, byte[]>) _fields).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<short, byte[]> item)
        {
            return ((IDictionary<short, byte[]>)_fields).Remove(item);
        }

        public int Count => _fields.Count;

        public bool IsReadOnly { get; } = false;

        public bool ContainsKey(short key)
        {
            return _fields.ContainsKey(key);
        }

        public bool Remove(short key)
        {
            return _fields.Remove(key);
        }

        public bool TryGetValue(short key, out byte[] value)
        {
            return _fields.TryGetValue(key, out value);
        }

        public byte[] this[short key]
        {
            get => _fields[key];
            set => _fields[key] = value;
        }

        public ICollection<short> Keys => _fields.Keys;
        public ICollection<byte[]> Values => _fields.Values;
    }
}