using System;
using System.IO;
using System.Collections.Generic;
using LS_FundParser.DataStorage;
using LS_FundParser.Mockup;
using System.Text.RegularExpressions;
using System.Net;

namespace LS_FundParser
{
    public class HtmlRetriever{

        public string GetPage(string url){
            var client = new WebClient();
            var result = client.DownloadString(url);
            client.Dispose();
            return result;
        }

        public void SavePage(string url, string outputFile){
            var content = GetPage(url);
            var writer = new StreamWriter(outputFile);
            writer.Write(content);
            writer.Flush();
            writer.Close();
        }

    }
}