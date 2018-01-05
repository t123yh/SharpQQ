namespace SharpQQ.Protocol.Jce
{
    internal enum JceStructType
    {
        BYTE = 0,
        SHORT = 1,
        INT = 2,
        LONG = 3,
        FLOAT = 4,
        DOUBLE = 5,
        STRING1 = 6,
        STRING4 = 7,
        MAP = 8,
        LIST = 9,
        STRUCT_BEGIN = 10,
        STRUCT_END = 11,
        ZERO_TAG = 12,
        SIMPLE_LIST = 13,
    }
}