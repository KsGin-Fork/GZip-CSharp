using System;

namespace GZIPmodel
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("不受认可的程序参数");
                return;
            }
            switch (args[0])
            {
                case "gzip":
                    try
                    {
                        GZIPFunction.GZIP(args[1], args[2]);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("输入参数有误");
                    }
                    break;
                case "ungzip":
                    try
                    {
                        GZIPFunction.UNGZIP(args[1], args[2]);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("输入参数有误");
                    }
                    break;
                default:
                    Console.WriteLine("找不到此命令");
                    break;
            }
        }
    }
}
