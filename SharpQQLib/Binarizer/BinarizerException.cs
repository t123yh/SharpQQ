using System;

namespace SharpQQ.Binarizer
{
    public class BinarizerException : Exception
    {
        public string FieldName { get; }

        public BinarizerException()
        {
        }

        public BinarizerException(string message, string fieldName = null) : base(message)
        {
            this.FieldName = fieldName;
        }

        public BinarizerException(string message, Exception inner, string fieldName) : base(message, inner)
        {
            this.FieldName = fieldName;
        }
    }
}