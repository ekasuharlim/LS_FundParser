using System;
using System.IO;
using System.Collections.Generic;
using LS_FundParser.DataStorage;
using LS_FundParser.Mockup;
using System.Text.RegularExpressions;

namespace LS_FundParser
{
    public class HtmlParser
    {
        public string Parse(string inputFile,string stockCode, IDataStorage outputStorage){
            var reader = new StreamReader(inputFile);
            var tempFile = $"{stockCode}.temp";
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
            InsertDataToDb(outputStorage,stockCode,data);

            return inputFile;
        }

        private List<List<string>> ParseTempResult(string inputFile){            
            var reader = new StreamReader(inputFile);
            var line = reader.ReadLine();
            var data = new List<List<string>>();
            List<string> detaillData = null;
            while(line != null){                
                line = line.ToUpper().Trim();
                line = line.Replace(",","");
                line = line.Replace(" ","_");
                if(line == "") {                    
                    detaillData = new List<string>();
                    data.Add(detaillData);   
                }else{
                    detaillData.Add(line);
                }
                
                line = reader.ReadLine();
            }

            reader.Close();
            return data;
        }        

        private bool InsertDataToDb(IDataStorage storage,string stockCode, List<List<string>> data){
            List<string> yearData = new List<string>();
            for(var idxGroup = 0; idxGroup < data.Count ; idxGroup++){
                 var groupName = data[idxGroup].Count > 0 ? data[idxGroup][0] : "";                           
                 if (groupName != ""){                    
                    if (groupName.Contains("ANLZ")){
                        yearData = data[idxGroup];
                    }else{
                        for(var idxValue = 1; idxValue < yearData.Count; idxValue++){                    
                            if (data[idxGroup].Count > yearData.Count){
                                var value = data[idxGroup][idxValue];                    
                                storage.Insert(stockCode,GetYear(yearData[idxValue]),GetGroupName(groupName),GetValue(value));
                            }                        
                        }
                    }
                }
            }
            return true;
        }

        private string GetYear(string yearName){
            var year = yearName.Substring(yearName.Length - 4, 4);
            return year;
        }

        private string GetValue(string value){
            value  = value.Replace("_","");
            value  = value.Replace(",",".");
            var groupAmount = value.Substring(value.Length - 1, 1);
            if (!Regex.IsMatch(groupAmount, @"^\d+$")){
                value  = value.Replace(groupAmount,"");
            }
            var amount =  Convert.ToDouble(value);
            switch (groupAmount){
                case "T":
                    amount = amount * 1000;
                    break;
                case "M":
                    amount = amount * 1000;
                    break;
            }
            return Convert.ToString(amount);
        }

        private string GetGroupName(string groupName){
            return groupName.Replace(".","_").Replace("/","_");
        }

    }

}
