using System;
using LS_FundParser.Mockup;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace LS_FundParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HtmlParser();
            var retriever = new HtmlRetriever();
            var fs = new FileStorage(@"Output\result.txt");
            var codeList = new List<string>{"UNVR","SIDO","BBCA"};

            var reader = new StreamReader(@"Data\Master\StockCodeList.txt");
            var code = reader.ReadLine();
            while(code != null){         
                code = code.Trim();
                if(!String.IsNullOrWhiteSpace(code)){
                    var rawData = $@"Data\{code}.txt"; 
                    Console.WriteLine(rawData);
                    retriever.SavePage($"https://www.indopremier.com/module/saham/include/fundamental.php?code={code}&quarter=4",rawData);
                    parser.Parse(rawData,code,fs);
                    Thread.Sleep(2);
                }   
                code = reader.ReadLine();
            }
            fs.Close();
            reader.Close();
            Console.WriteLine("Done!");
        }
    }
}
