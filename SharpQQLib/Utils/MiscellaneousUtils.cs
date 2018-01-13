using System;

namespace SharpQQ.Utils
{
    public class MiscellaneousUtils
    {
        static Random _rnd = new Random();

        internal static byte RandomByte()
        {
            return 0;
        }
        
        internal static int UnifiedRandomInt()
        {
            return 0x12345678;
        }

        internal static byte[] UnifiedRandomBytes16()
        {
            return new byte[] { 2, 3, 4, 5, 6, 7, 8, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        }

        internal static byte[] UnifiedRandomBytes(int length)
        {
            byte[] result = new byte[length];
            _rnd.NextBytes(result);
            return result;
        }

    }
}