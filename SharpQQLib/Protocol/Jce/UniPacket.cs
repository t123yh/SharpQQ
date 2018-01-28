using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using SharpQQ.Utils;

namespace SharpQQ.Protocol.Jce
{
    // The following code is a piece of shit.
    // Don't look at it, just use it.

    public class UniPacket
    {
        public UniPacketStruct Packet { get; set; } = new UniPacketStruct();

        private Dictionary<string, byte[]> _data = new Dictionary<string, byte[]>();

        public UniPacket(string servant, string function)
        {
            this.Packet.ServantName = servant;
            this.Packet.FuncName = function;
            this.Packet.Context = new Dictionary<string, string>();
            this.Packet.Status = new Dictionary<string, string>();
        }

        public UniPacket(byte[] buf)
        {
            this.Deocde(buf);
        }

        public void Put(string name, dynamic thing)
        {
            var stream = new JceOutputStream();
            stream.Write(thing, 0);
            _data.Add(name, stream.toByteArray());
        }

        public T Get<T>(string name)
        {
            var stream = new JceInputStream(this._data[name]);
            return (T) stream.Read(typeof(T), null, 0, true);
        }

        public byte[] Encode()
        {
            var _os = new JceOutputStream();
            _os.Write(this._data, 0);
            this.Packet.Buffer = _os.toByteArray();
            var _os2 = new JceOutputStream();
            this.Packet.WriteTo(_os2);
            return _os2.toByteArray();
        }

        private void Deocde(byte[] buf)
        {
            var _is = new JceInputStream(buf);
            this.Packet = new UniPacketStruct();
            this.Packet.ReadFrom(_is);
            var _is2 = new JceInputStream(this.Packet.Buffer);
            if (this.Packet.Version == 3)
            {
                this._data =
                    (Dictionary<string, byte[]>) _is2.Read(typeof(Dictionary<string, byte[]>), this._data, 0, true);
            }
            else if (this.Packet.Version == 2)
            {
                this._data = _is2.readMap<Dictionary<string, Dictionary<string, byte[]>>>(0, false)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.First().Value);
            }
            else
            {
                throw new JceDecodeException($"Incompatible UniPacket version: {this.Packet.Version}");
            }
        }
    }
}