using System;
using System.IO;
using System.Collections.Generic;
using LS_FundParser.DataStorage;
using LS_FundParser.Mockup;

namespace LS_FundParser
{
    public class HtmlParser
    {
        public string Parse(string inputFile, string outputFile){
            var reader = new StreamReader(inputFile);
            var tempFile = outputFile + ".temp";
            var writer = new StreamWriter(tempFile);
            var line = reader.ReadLine();
            var skip  = true;
            while(line != null){                
                skip  = true;
                line = line.ToUpper();
                line = line.Replace("\n", "").Replace("\r", "").Replace("\t", "");                
                if(line.Contains("<TR") || line.Contains("<TD")  || line.Contains("<TH") ) skip = false;
                if(line == "<THEAD>")  skip = true;                
                if(line == "<TH ALIGN=\"LEFT\">")  skip = true;                
                if(line.Contains("BUTTON"))  skip = true;
                if(line.Contains("<TD COLSPAN=\"10\" ALIGN=\"LEFT\"><STRONG>"))  skip = true;

                if(!skip){
                    line = line.Replace("<TD COLSPAN=\"2\" ALIGN=\"LEFT\">","<TD ALIGN=\"RIGHT\">");                
                    line = line.Replace("<TH ALIGN=\"RIGHT\">","");
                    line = line.Replace("</TH>",",");
                    line = line.Replace("<TD ALIGN=\"RIGHT\">","");
                    line = line.Replace("</TD>",",");
                    line = line.Replace("<TR>","");

                    writer.WriteLine(line);

                    
                }
                line = reader.ReadLine();
            }
            writer.Flush();
            reader.Close();
            writer.Close();
            
            var data =  ParseTempResult(tempFile);
            var fs = new FileStorage(outputFile);
            InsertDataToDb(fs,"SIDO",data);
            fs.Close();

            return outputFile;

        }

        private bool InsertDataToDb(IDataStorage storage,string stockCode, List<List<string>> data){
            var yearData = data[0];            
            
            for(var idxGroup = 1; idxGroup < data.Count ; idxGroup++){
                Console.WriteLine("");
                // var groupName = data[idxGroup][1];                
                // Console.WriteLine(groupName);
                 for(var idxValue = 1; idxValue < data[idxGroup].Count; idxValue++){                    
                //     if (data[idxGroup].Count > yearData.Count){
                //         var value = data[idxGroup][idxValue];                    
                //         storage.Insert(stockCode,yearData[idxValue],groupName,value);
                //     }
                    Console.Write(data[idxGroup][idxValue]);
                }

            }

            return true;
        }

        private List<List<string>> ParseTempResult(string inputFile){            
            var reader = new StreamReader(inputFile);
            var line = reader.ReadLine();
            var data = new List<List<string>>();
            List<string> detaillData = null;
            while(line != null){                
                line = line.ToUpper().Trim();
                line = line.Replace(",","");
                line = line.Replace(" ","");
                if(line == "") {
                    detaillData = new List<string>();
                    data.Add(detaillData);   
                }
                detaillData.Add(line);
                line = reader.ReadLine();
            }

            reader.Close();
            return data;
        }
    }

}
