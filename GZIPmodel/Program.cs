using System;
using System.Collections.Generic;

namespace GZIPmodel
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args[0] == "gzip")
            {
                try
                {
                    GZIPFunction.GZIP(args[1], args[2]);
                }
                catch (Exception)
                {
                    Console.WriteLine("输入参数有误");
                }
            }
            else if (args[0] == "ungzip")
            {
                try
                {
                    GZIPFunction.UNGZIP(args[1], args[2]);
                }
                catch (Exception)
                {
                    Console.WriteLine("输入参数有误");
                }
            }
            else
            {
                Console.WriteLine("找不到此命令");
            }
            
            Console.Read();
        }
    }
}
