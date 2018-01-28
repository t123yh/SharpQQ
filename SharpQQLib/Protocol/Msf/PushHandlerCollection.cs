using System;
using System.Collections.Generic;

namespace SharpQQ.Protocol.Msf
{
    public class PushHandlerCollection
    {
        private readonly Dictionary<string, EventHandler<PushEventArgs>> _innerDictionary;

        public PushHandlerCollection()
        {
            _innerDictionary = new Dictionary<string, EventHandler<PushEventArgs>>();
        }

        public EventHandler<PushEventArgs> this[string eventName]
        {
            get
            {
                if (_innerDictionary.ContainsKey(eventName))
                    return _innerDictionary[eventName];
                else
                    return null;
            }
            set
            {
                if (value == null && _innerDictionary.ContainsKey(eventName))
                    _innerDictionary.Remove(eventName);
                else
                    _innerDictionary[eventName] = value;
            }
        }

        public void Invoke(string name, object sender, PushEventArgs args)
        {
            if (_innerDictionary.ContainsKey(name))
                _innerDictionary[name].Invoke(sender, args);
        }
    }

    public class PushEventArgs : EventArgs
    {
        public string Name { get; }

        public byte[] Data { get; }

        public PushEventArgs(string name, byte[] data)
        {
            this.Name = name;
            this.Data = data;
        }
    }
}