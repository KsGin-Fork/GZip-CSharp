using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GZIPmodel
{
    internal class GZIPFunction
    {

        /// <summary>
        /// 获取文本的编码格式
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Encoding GetFileEncodeType(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(2);
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return Encoding.UTF8;
                }
                if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return Encoding.BigEndianUnicode;
                }
                if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return Encoding.Unicode;
                }
                return Encoding.Default;
            }
            return Encoding.Default;
        }


        /// <summary>
        /// 转换非unicode文本为unicode文本
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static void EncodingToUnicode(string path)
        {
            var sb = new StringBuilder();
            using (var sr = new StreamReader(path , GetFileEncodeType(path) , false))
            {
                 sb.Append(sr.ReadToEnd());
            }
            


        }

        /// <summary>
        /// 由八位字符串转化为无符号整形数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static byte StringToUint(string str)
        {
            byte number = 0;
            if (str.Length < 8)          //不足八位补足八位
            {
                for (var i = str.Length; i < 8; i++)
                {
                    str = str + "0";
                }
            }
            for (var i = 0; i < 8; i++)
            {
                if (str[i] == '1')
                {
                    number += (byte)Math.Pow(2 ,  8 - i - 1);
                }
            }
            return number;
        }

        /// <summary>
        /// 由16位二进制字符串转化为unicode字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static int StringToChar(string str)
        {
            var number = 0;
            if (str.Length < 16)          //不足16位补足16位
            {
                for (var i = str.Length; i < 16; i++)
                {
                    str = str + "0";
                }
            }
            for (var i = 0; i < 16; i++)
            {
                if (str[i] == '1')
                {
                    number += (int)Math.Pow(2 , 16 - i - 1);
                }
            }
            return number;
        }


        /// <summary>
        /// byte转化为二进制字符串
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static string DtoB(uint b)
        {
            var str = "";
            while (b/2 != 0)
            {
                str = b % 2 + str;
                b = b/2;
            }
            str = b%2 + str;
            if (str.Length >= 8) return str;
            for (var i = str.Length; i < 8; i++)
            {
                str = "0" + str;
            }
            return str;
        }

        /// <summary>
        ///     压缩函数
        /// </summary>
        /// <param name="readFilePath">要压缩的文件全路径</param>
        /// <param name="writeFilePath">要存储的文件全路径</param>
        public static void GZIP(string readFilePath, string writeFilePath)
        {
            
            using (var streamReader = new StreamReader(readFilePath , GetFileEncodeType(readFilePath) , false))
            {
                var str = streamReader.ReadToEnd();
                Regex.Replace(str, "[\r\n]", "\n");
                var huff = new GZIPhuffman(str);
                var b = huff.GZIPcoding(str);
                var bytes = new List<byte>();
                for (var i = 0; i < b.Length / 8; i++)
                {
                    var tmp = StringToUint(b.Substring(i * 8, 8));
                    try
                    {
                        bytes.Add(tmp);
                    }
                    catch (Exception)
                    {
                        throw new Exception();
                    }
                }
                File.WriteAllBytes(writeFilePath, bytes.ToArray());   
                Console.WriteLine("压缩完毕");
            }
        }
                
        /// <summary>
        ///     解压函数
        /// </summary>
        /// <param name="readFilePath">要解压的文件全路径</param>
        /// <param name="writeFilePath">要存储的文件全路径</param>
        public static void UNGZIP(string readFilePath, string writeFilePath)
        {
            var bytes = File.ReadAllBytes(readFilePath);
            const string Gzipflag = "000000000010100000000000011001110000000001111010000000000110100100000000011100000000000000101001";       //压缩标志
            var code = bytes.Aggregate("", (current, t) => current + DtoB(t));
            var strs = Regex.Split(code, Gzipflag);
            //Console.WriteLine(code.Length + " = " + strs[0].Length + "\r\n\r\n\r\n" + strs[1].Length);
            var oldStrPar = strs[0];
            var Par = "";
            for (var i = 0; i < oldStrPar.Length / 16; i++)
            {
                var tmp = StringToChar(oldStrPar.Substring(i * 16, 16));
                try
                {
                    Par += (char)tmp;
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }

            Dictionary<char , uint> mDictionary = new Dictionary<char, uint>();
            var match = Regex.Match(Par, "(.{0,1}|\\n):(\\d+)\\|");          //通过正则匹配huffman树的key和value
            while (match.Success)
            { 
                var Key = match.Groups[1].Value[0];
                var Value = uint.Parse(match.Groups[2].Value);
                mDictionary.Add(Key , Value);                           //构造字典
                match = match.NextMatch();
            }
            var gzi = new GZIPhuffman(mDictionary);
            var text = gzi.GZIPtranslate(strs[1]);
            Regex.Replace(text, "[\n]", "\r\n");
            File.WriteAllText(writeFilePath, text);
            Console.WriteLine("解压完毕");
        }
    }
}
