using LS_FundParser.DataStorage;
using System.IO;
namespace LS_FundParser.Mockup{
    public class FileStorage : IDataStorage
    {
        StreamWriter _writer;
        public FileStorage(string filePath){
            _writer  = new StreamWriter(filePath);
        }
        public void Insert(string code, string year, string group, string value)
        {
            _writer.WriteLine($"{code}-{group}-{year}-{value}");
        }

        public void Close(){
            _writer.Flush();
            _writer.Close();
        }
    }
}