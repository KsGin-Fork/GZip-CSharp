using System;
using System.Collections.Generic;

namespace GZIPmodel
{
    internal class Program
    {
        private static void Main()
        {
            GZIPFunction.GZIP(@"C: \Users\KsGin\Desktop\Test1.txt" , @"C: \Users\KsGin\Desktop\Test2.gzip");
            GZIPFunction.UNGZIP(@"C: \Users\KsGin\Desktop\Test2.gzip", @"C: \Users\KsGin\Desktop\Test3.txt");
            Console.Read();
        }
    }
}
