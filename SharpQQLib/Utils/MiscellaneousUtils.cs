using System;

namespace SharpQQ.Utils
{
    public class MiscellaneousUtils
    {
        private static Random _rnd = new Random();

        internal static byte RandomByte()
        {
#if DEBUG
            return 0;
#else
            byte[] b = new byte[1];
            return _rnd.Next(b);
            return b[0];
#endif
        }

        internal static int UnifiedRandomInt()
        {
#if DEBUG
            return 0x12345678;
#else
            return _rnd.Next();
#endif
        }

        internal static byte[] UnifiedRandomBytes(int length)
        {
            byte[] result = new byte[length];
#if DEBUG
            for (int i = 0; i < length; i++)
            {
                result[i] = (byte) i;
            }
#else
            _rnd.NextBytes(result);
#endif
            return result;
        }
    }
}