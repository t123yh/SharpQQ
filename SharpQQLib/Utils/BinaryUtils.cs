using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SharpQQ.Utils
{
    public enum PrependLengthType
    {
        None,
        Int8,
        Int16,
        Int32
    }
    

    public static class BinaryUtils
    {
        static MD5 _md5 = new MD5CryptoServiceProvider();

        public static byte[] ComputeMD5(byte[] src)
        {
            byte[] ret = _md5.ComputeHash(src);
            return ret;
        }

        public static byte[] ComputeMD5(string src)
        {
            return _md5.ComputeHash(Encoding.ASCII.GetBytes(src));
        }

        public static byte[] HexToBin(string hexRaw)
        {
            string hex = hexRaw.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");
            byte[] dat = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                dat[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return dat;
        }

        public static string BinToHex(byte[] bin)
        {
            StringBuilder sb = new StringBuilder(bin.Length * 2);
            for (int i = 0; i < bin.Length; i++)
            {
                sb.Append(bin[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
