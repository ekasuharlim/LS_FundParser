using System;

namespace LS_FundParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HtmlParser();
            parser.Parse(@"Data\SIDO.txt",@"Output\SidoTemp.txt");
            Console.WriteLine("Hello World!");
        }
    }
}
