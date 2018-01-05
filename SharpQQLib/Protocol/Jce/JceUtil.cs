/**
 * Tencent is pleased to support the open source community by making Tars available.
 *
 * Copyright (C) 2016THL A29 Limited, a Tencent company. All rights reserved.
 *
 * Licensed under the BSD 3-Clause License (the "License"); you may not use this file except 
 * in compliance with the License. You may obtain a copy of the License at
 *
 * https://opensource.org/licenses/BSD-3-Clause
 *
 * Unless required by applicable law or agreed to in writing, software distributed 
 * under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 * CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 * specific language governing permissions and limitations under the License.
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpQQ.Protocol.Jce
{
    public class BasicClassTypeUtil
    {
        /**
         * 将嵌套的类型转成字符串
         * @param listTpye
         * @return
         */
        public static string TransTypeList(List<string> listTpye)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < listTpye.Count; i++)
            {
                listTpye[i] = CS2UniType(listTpye[i]);
            }

            listTpye.Reverse();

            for (int i = 0; i < listTpye.Count; i++)
            {
                string type = (string)listTpye[i];

                if (type == null)
                {
                    continue;
                }

                if (type.Equals("list"))
                {
                    listTpye[i - 1] = "<" + listTpye[i - 1];
                    listTpye[0] = listTpye[0] + ">";
                }
                else if (type.Equals("map"))
                {
                    listTpye[i - 1] = "<" + listTpye[i - 1] + ",";
                    listTpye[0] = listTpye[0] + ">";
                }
                else if (type.Equals("Array"))
                {
                    listTpye[i - 1] = "<" + listTpye[i - 1];
                    listTpye[0] = listTpye[0] + ">";
                }
            }
            listTpye.Reverse();
            foreach (string s in listTpye)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        public static object CreateObject<T>()
        {
            return CreateObject(typeof(T));
        }

        public static object CreateObject(Type type)
        {
            try
            {
                // String类型没有缺少构造函数，
                if (type.ToString() == "System.String")
                {
                    return "";
                }
                else if (type == typeof(byte[]))
                {
                    return new byte[0];
                }
                else if (type == typeof(short[]))
                {
                    return new short[0];
                }
                else if (type == typeof(ushort[]))
                {
                    return new ushort[0];
                }
                else if (type == typeof(int[]))
                {
                    return new int[0];
                }
                else if (type == typeof(uint[]))
                {
                    return new uint[0];
                }
                else if (type == typeof(long[]))
                {
                    return new long[0];
                }
                else if (type == typeof(ulong[]))
                {
                    return new ulong[0];
                }
                else if (type == typeof(char[]))
                {
                    return new char[0];
                }

                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object CreateListItem(Type typeList)
        {
            Type[] itemType = typeList.GetGenericArguments();
            if (itemType == null || itemType.Length == 0)
            {
                return null;
            }
            return CreateObject(itemType[0]);
        }

        public static string CS2UniType(string srcType)
        {
            if (srcType.Equals("System.Int16"))
            {
                return "short";
            }
            else if (srcType.Equals("System.UInt16"))
            {
                return "ushort";
            }
            else if (srcType.Equals("System.Int32"))
            {
                return "int32";
            }
            else if (srcType.Equals("System.UInt32"))
            {
                return "uint32";
            }
            else if (srcType.Equals("System.Boolean"))
            {
                return "bool";
            }
            else if (srcType.Equals("System.Byte"))
            {
                return "char";
            }
            else if (srcType.Equals("System.Double"))
            {
                return "double";
            }
            else if (srcType.Equals("System.Single"))
            {
                return "float";
            }
            else if (srcType.Equals("System.Int64"))
            {
                return "int64";
            }
            else if (srcType.Equals("System.UInt64"))
            {
                return "uint64";
            }
            else if (srcType.Equals("System.String"))
            {
                return "string";
            }
            else if (srcType.IndexOf("System.Collections.Generic.QDictionary") == 0)
            {
                return "map";
            }
            else if (srcType.IndexOf("System.Collections.Generic.List") == 0)
            {
                return "list";
            }
            else
            {
                return srcType;
            }
        }

        public static bool IsQDictionary(string cls)
        {
            return cls.IndexOf("System.Collections.Generic.QDictionary") == 0;
        }
    }

    internal class ByteConverter
    {
        public static byte[] ReverseBytes(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }

        public static short ReverseEndian(short value)
        {
            return BitConverter.ToInt16(ReverseBytes(BitConverter.GetBytes(value)), 0);
        }

        public static ushort ReverseEndian(ushort value)
        {
            return BitConverter.ToUInt16(ReverseBytes(BitConverter.GetBytes(value)), 0);
        }

        public static int ReverseEndian(int value)
        {
            return BitConverter.ToInt32(ReverseBytes(BitConverter.GetBytes(value)), 0);
        }

        public static uint ReverseEndian(uint value)
        {
            return BitConverter.ToUInt32(ReverseBytes(BitConverter.GetBytes(value)), 0);
        }

        public static long ReverseEndian(long value)
        {
            return BitConverter.ToInt64(ReverseBytes(BitConverter.GetBytes(value)), 0);
        }

        public static ulong ReverseEndian(ulong value)
        {
            return BitConverter.ToUInt64(ReverseBytes(BitConverter.GetBytes(value)), 0);
        }

        public static float ReverseEndian(float value)
        {
            return BitConverter.ToSingle(ReverseBytes(BitConverter.GetBytes(value)), 0);
        }

        public static double ReverseEndian(double value)
        {
            return BitConverter.ToDouble(ReverseBytes(BitConverter.GetBytes(value)), 0);
        }

        public static string Bytes2String(byte[] bytes)
        {
            string strOutput = "";

            int nLenWithoutZero = 0;
            for (; nLenWithoutZero < bytes.Length; nLenWithoutZero++)
            {
                if (bytes[nLenWithoutZero] == 0)
                {
                    break;
                }
            }
            byte[] byteWithoutZero = new byte[nLenWithoutZero];

            Array.Copy(bytes, byteWithoutZero, byteWithoutZero.Length);

            List<int> listIndexes = new List<int>();
            for (int i = 0; i < byteWithoutZero.Length - 1; i++)
            {
                // 检查字符串为零
                if (byteWithoutZero[i] == 0)
                {
                    break;
                }
                if (byteWithoutZero[i] == 0x14)
                {
                    listIndexes.Add(i);
                    i++;
                }
            }

            if (listIndexes.Count > 0)
            {
                if (listIndexes[0] > 0)
                {
                    strOutput += Encoding.UTF8.GetString(byteWithoutZero, 0, listIndexes[0]);
                }

                strOutput += (char)byteWithoutZero[listIndexes[0]];
                strOutput += (char)byteWithoutZero[listIndexes[0] + 1];
            }

            for (int i = 1; i < listIndexes.Count; i++)
            {
                int num = listIndexes[i] - listIndexes[i - 1] - 2;
                if (num > 0)
                {
                    strOutput += Encoding.UTF8.GetString(byteWithoutZero, listIndexes[i - 1] + 2, num);
                }

                strOutput += (char)byteWithoutZero[listIndexes[i]];
                strOutput += (char)byteWithoutZero[listIndexes[i] + 1];
            }


            int leftIndex = 0;
            if (listIndexes.Count > 0)
            {
                leftIndex = listIndexes[listIndexes.Count - 1] + 2;
            }
            if (leftIndex < byteWithoutZero.Length)
            {
                strOutput += Encoding.UTF8.GetString(byteWithoutZero, leftIndex, byteWithoutZero.Length - leftIndex);
            }
            return strOutput;
        }

        public static bool IsCharValidate(char ch)
        {
            byte high = (byte)((ch >> 8) & 0xff);
            byte low = (byte)(ch & 0xff);
            if (high == 0 && ((low & 0x80) != 0))
            {
                return false;
            }
            return true;

        }
        /// <summary>
        /// 写本地文件是使用
        /// </summary>
        /// <param name="strInput"></param>
        /// <param name="IsLocalString"></param>
        /// <returns></returns>
        public static byte[] String2Bytes(string strInput, bool IsLocalString)
        {
            if (!IsLocalString)
            {
                return String2Bytes(strInput);
            }
            else
            {
                return Encoding.UTF8.GetBytes(strInput);
            }
        }

        public static byte[] String2Bytes(string strInput)
        {
            if (strInput == null)
            {
                return null;
            }
            char[] chars = strInput.ToCharArray();

            List<int> listIndexes = new List<int>();
            for (int i = 0; i < chars.Length; i++)
            {
                if (!IsCharValidate(chars[i]))
                {
                    listIndexes.Add(i);
                }
            }

            byte[] bytes = new byte[Encoding.UTF8.GetByteCount(strInput)];
            byte[] temp = null;
            int index = 0;

            if (listIndexes.Count > 0)
            {
                if (listIndexes[0] > 0)
                {
                    temp = Encoding.UTF8.GetBytes(chars, 0, listIndexes[0]);
                    Array.Copy(temp, 0, bytes, 0, temp.Length);
                    index += temp.Length;
                }

                bytes.SetValue((byte)chars[listIndexes[0]], index);
                index += 1;
            }

            for (int i = 1; i < listIndexes.Count; i++)
            {
                int num = listIndexes[i] - listIndexes[i - 1] - 1;
                if (num > 0)
                {
                    temp = Encoding.UTF8.GetBytes(chars, listIndexes[i - 1] + 1, num);
                    Array.Copy(temp, 0, bytes, index, temp.Length);
                    index += temp.Length;
                }

                bytes.SetValue((byte)chars[listIndexes[i]], index);
                index += 1;
            }


            int leftIndex = 0;
            if (listIndexes.Count > 0)
            {
                leftIndex = listIndexes[listIndexes.Count - 1] + 1;
            }
            if (leftIndex < bytes.Length)
            {
                temp = Encoding.UTF8.GetBytes(chars, leftIndex, chars.Length - leftIndex);
                Array.Copy(temp, 0, bytes, index, temp.Length);
                index += temp.Length;
            }

            byte[] output = new byte[index];
            Array.Copy(bytes, output, index);
            return output;
        }
    }
    internal class JceUtil
    {

        /**
         * Constant to use in building the hashCode.
         */
        private static int iConstant = 37;

        /**
         * Running total of the hashCode.
         */
        private static int iTotal = 17;

        public static bool Equals(bool l, bool r)
        {
            return l == r;
        }

        public static bool Equals(byte l, byte r)
        {
            return l == r;
        }

        public static bool Equals(char l, char r)
        {
            return l == r;
        }

        public static bool Equals(short l, short r)
        {
            return l == r;
        }

        public static bool Equals(int l, int r)
        {
            return l == r;
        }

        public static bool Equals(long l, long r)
        {
            return l == r;
        }

        public static bool Equals(float l, float r)
        {
            return l == r;
        }

        public static bool Equals(double l, double r)
        {
            return l == r;
        }

        public static new bool Equals(object l, object r)
        {
            return l.Equals(r);
        }

        public static int compareTo(bool l, bool r)
        {
            return (l ? 1 : 0) - (r ? 1 : 0);
        }

        public static int compareTo(byte l, byte r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int compareTo(char l, char r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int compareTo(short l, short r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int compareTo(int l, int r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int compareTo(long l, long r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int compareTo(float l, float r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int compareTo(double l, double r)
        {
            return l < r ? -1 : (l > r ? 1 : 0);
        }

        public static int compareTo<T>(T l, T r) where T : IComparable
        {
            return l.CompareTo(r);
        }

        public static int compareTo<T>(List<T> l, List<T> r) where T : IComparable
        {
            int n = 0;
            for (int i = 0, j = 0; i < l.Count && j < r.Count; i++, j++)
            {
                if (l[i] is IComparable && r[j] is IComparable)
                {
                    IComparable lc = (IComparable)l[i];
                    IComparable rc = (IComparable)r[j];
                    n = lc.CompareTo(rc);
                    if (n != 0)
                    {
                        return n;
                    }
                }
                else
                {
                    throw new Exception("Argument must be IComparable!");
                }
            }

            return compareTo(l.Count, r.Count);
        }

        public static int compareTo<T>(T[] l, T[] r) where T : IComparable
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = l[li].CompareTo(r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int compareTo(bool[] l, bool[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = compareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int compareTo(byte[] l, byte[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = compareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int compareTo(char[] l, char[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = compareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int compareTo(short[] l, short[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = compareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int compareTo(int[] l, int[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = compareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int compareTo(long[] l, long[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = compareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int compareTo(float[] l, float[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = compareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int compareTo(double[] l, double[] r)
        {
            for (int li = 0, ri = 0; li < l.Length && ri < r.Length; ++li, ++ri)
            {
                int n = compareTo(l[li], r[ri]);
                if (n != 0)
                    return n;
            }
            return compareTo(l.Length, r.Length);
        }

        public static int hashCode(bool o)
        {
            return iTotal * iConstant + (o ? 0 : 1);
        }

        public static int hashCode(bool[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + (array[i] ? 0 : 1);
                }
                return tempTotal;
            }
        }

        public static int hashCode(byte o)
        {
            return iTotal * iConstant + o;
        }

        public static int hashCode(byte[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + array[i];
                }
                return tempTotal;
            }
        }

        public static int hashCode(char o)
        {
            return iTotal * iConstant + o;
        }

        public static int hashCode(char[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + array[i];
                }
                return tempTotal;
            }
        }

        public static int hashCode(double o)
        {
            return hashCode(Convert.ToInt64(o));
        }

        public static int hashCode(double[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + ((int)(Convert.ToInt64(array[i]) ^ (Convert.ToInt64(array[i]) >> 32)));
                }
                return tempTotal;
            }
        }

        public static int hashCode(float o)
        {
            return iTotal * iConstant + Convert.ToInt32(o);
        }

        public static int hashCode(float[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + Convert.ToInt32(array[i]);
                }
                return tempTotal;
            }
        }

        public static int hashCode(short o)
        {
            return iTotal * iConstant + o;
        }

        public static int hashCode(short[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + array[i];
                }
                return tempTotal;
            }
        }


        public static int hashCode(int o)
        {
            return iTotal * iConstant + o;
        }

        public static int hashCode(int[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + array[i];
                }
                return tempTotal;
            }
        }

        public static int hashCode(long o)
        {
            return iTotal * iConstant + ((int)(o ^ (o >> 32)));
        }

        public static int hashCode(long[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + ((int)(array[i] ^ (array[i] >> 32)));
                }
                return tempTotal;
            }
        }

        public static int hashCode(JceStruct[] array)
        {
            if (array == null)
            {
                return iTotal * iConstant;
            }
            else
            {
                int tempTotal = iTotal;
                for (int i = 0; i < array.Length; i++)
                {
                    tempTotal = tempTotal * iConstant + (array[i].GetHashCode());
                }
                return tempTotal;
            }
        }


        public static int hashCode(object obj)
        {
            if (null == obj)
            {
                return iTotal * iConstant;
            }
            else
            {
                if (obj.GetType().IsArray)
                {
                    if (obj is long[])
                    {
                        return hashCode((long[])obj);
                    }
                    else if (obj is int[])
                    {
                        return hashCode((int[])obj);
                    }
                    else if (obj is short[])
                    {
                        return hashCode((short[])obj);
                    }
                    else if (obj is char[])
                    {
                        return hashCode((char[])obj);
                    }
                    else if (obj is byte[])
                    {
                        return hashCode((byte[])obj);
                    }
                    else if (obj is double[])
                    {
                        return hashCode((double[])obj);
                    }
                    else if (obj is float[])
                    {
                        return hashCode((float[])obj);
                    }
                    else if (obj is bool[])
                    {
                        return hashCode((bool[])obj);
                    }
                    else if (obj is JceStruct[])
                    {
                        return hashCode((JceStruct[])obj);
                    }
                    else
                    {
                        return hashCode((Object[])obj);
                    }
                }
                else if (obj is JceStruct)
                {
                    return obj.GetHashCode();
                }
                else
                {
                    return iTotal * iConstant + obj.GetHashCode();
                }
            }
        }

        public static byte[] getTarsBufArray(MemoryStream ms)
        {
            byte[] bytes = new byte[ms.Position];
            Array.Copy(ms.GetBuffer(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}