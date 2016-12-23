using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZIPmodel
{
    internal class Program
    {
        private static void Main()
        {
            GZIPFunction.GZIP(@"C: \Users\KsGin\Desktop\Test.txt" , @"C: \Users\KsGin\Desktop\Test.gzip");
            GZIPFunction.UNGZIP(@"C: \Users\KsGin\Desktop\Test.gzip" , @"C: \Users\KsGin\Desktop\Test1.txt");
            //GZIPhuffman g = new GZIPhuffman("你好啊");
            //Console.WriteLine(g.GZIPtranslate(g.GZIPcoding("你好啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊")));
            Console.Read();
        }
    }
}
