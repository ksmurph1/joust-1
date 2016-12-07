using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace DL
{
    public struct CsvReader : ICsvReader
    {
        public string GetNextLatestCsv()
        {
            string dataDir=ConfigurationManager.AppSettings["data-dir"];
            object[] dates= (from file in Directory.GetFiles(dataDir,"*.csv")
             let r=new Regex(@"\w+\.(\d{4}\.\d{2}\.\d{2})",RegexOptions.IgnoreCase|RegexOptions.Compiled)
             where (r.IsMatch(file))
             select (new {file=file,date=r.Match(file).Value}).ToArray();
        }
          public IInventory ParseFile(string filename)
        {
            string line;
            using (StreamReader reader=new StreamReader(filename,System.Text.Encoding.ASCII))
            {
            while ((line=reader.ReadLine()) != null)
            {
               line.Split(',');
            }
            }
        }
    }
}
