using System;
using System.Collections.Generic;
using System.IO;
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
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);
            var buffer = br.ReadBytes(2);
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    fs.Close();
                    br.Close();
                    return Encoding.UTF8;
                }
                if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    fs.Close();
                    br.Close();
                    return Encoding.BigEndianUnicode;
                }
                if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    fs.Close();
                    br.Close();
                    return Encoding.Unicode;
                }
                fs.Close();
                br.Close();
                return Encoding.Default;
            }
            fs.Close();
            br.Close();
            return Encoding.Default;
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
            //用于存储压缩信息和存储压缩码的数据结构
            var codingpar = new Dictionary<char, uint>();
            var bytes = new List<byte>();

            string str;
            //读取文件
            using (var sr = new StreamReader(readFilePath, GetFileEncodeType(readFilePath), false))
            {
                str = sr.ReadToEnd();
                sr.Close();
            }
            //将文中的windows换行标准格式\r\n 替换为 \n
            str = Regex.Replace(str, "\r\n", "\n");

            //使用文本文件读取的字符串构造huffman
            var huff = new GZIPhuffman(str);
            var b = huff.GZIPcoding(str, ref codingpar);    //获得解压码  这里有大量的时间消耗

            //将每八个字节的01字符串编码转化成一个字节
            for (var i = 0; i < b.Length / 8 ; i++)
            {
                var tmp = StringToUint(b.Substring(i*8, 8));
                try
                {
                    bytes.Add(tmp);
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
            bytes.Add(StringToUint(b.Substring(b.Length - b.Length % 8)));


            //制作解压参数
            var codingParStr = new StringBuilder();
            foreach (var v in codingpar)
                codingParStr.Append(v.Key + ":" + v.Value + "|");
            codingParStr.Append("\r\n");
            //存入文件
            var fs = new FileStream(writeFilePath, FileMode.Create);
            var bs = new BufferedStream(fs);
            var bw = new BinaryWriter(bs , Encoding.Default);
            bw.Write(codingParStr.ToString());
            bw.Write(bytes.ToArray());
            bw.Flush();
            bw.Close();
            bs.Close();
            fs.Close();
        }
                
        /// <summary>
        ///     解压函数
        /// </summary>
        /// <param name="readFilePath">要解压的文件全路径</param>
        /// <param name="writeFilePath">要存储的文件全路径</param>
        public static void UNGZIP(string readFilePath, string writeFilePath)
        {
            var encoding = GetFileEncodeType(readFilePath);
            var fs = new FileStream(readFilePath, FileMode.Open);
            var bs = new BufferedStream(fs);
            var br = new BinaryReader(bs , encoding);

            //获取编码参数部分
            var sbPar = new StringBuilder();
            var ch = br.ReadChar();
            while (ch != '\r' || br.ReadChar() != '\n')
            {
                sbPar.Append(ch);
                ch = br.ReadChar();
            }
            //获取正式编码部分
            var sbCod = new StringBuilder();
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                sbCod.Append(DtoB(br.ReadByte()));
            }
            //关闭流
            br.Close();
            bs.Close();
            fs.Close();

            //制作编码参数数据权值字典
            var valueWeight = new Dictionary<char, uint>();
            var match = Regex.Match(sbPar.ToString() , "(.{0,1}|\\n|\\r):(\\d+)\\|");
            while (match.Success)
            {
                valueWeight.Add(match.Groups[1].Value[0] , uint.Parse(match.Groups[2].Value));
                match = match.NextMatch();
            }

            //权值字典建立huffman
            var huff = new GZIPhuffman(valueWeight);

            var result = huff.GZIPdecoding(sbCod.ToString());
            result = Regex.Replace(result, "\n", "\r\n");
            
            //写入应用
            var ifs = new FileStream(writeFilePath , FileMode.Create , FileAccess.Write);
            var ibs = new BufferedStream(ifs);
            var isw = new StreamWriter(ibs);
            isw.Write(result);
            isw.Close();
            ibs.Close();
            ifs.Close();
        }
    }
}
