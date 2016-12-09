using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;
namespace DL
{
    public struct CsvReader : ICsvReader
    {
        public string GetNextLatestCsv()
        {
            Regex rgx=new Regex(@"(\w+)\.(\d{4}\.\d{2}\.\d{2})",
                              RegexOptions.IgnoreCase|RegexOptions.Compiled);
            string dataDir=ConfigurationManager.AppSettings["data-dir"];
            
             var pieces= (from file in Directory.GetFiles(dataDir,"*.csv")
             let match=rgx.Match(file)
             where (match.Success)
             select new {f=file, coname=match.Captures[0].Value,
             date=DateTime.ParseExact(match.Captures[1].Value,"yyyy.MM.dd",
             System.Globalization.CultureInfo.InvariantCulture)}).
             OrderByDescending(o=>o.date).GroupBy(o=>o.coname).AsParallel().ToArray();

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
