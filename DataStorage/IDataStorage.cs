using System;

namespace LS_FundParser.DataStorage
{
     public interface IDataStorage{
         void Insert(string code, string year, string group, string value);
     }  
}