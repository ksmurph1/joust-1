using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System;
using Util;
namespace DL
{
    public struct CsvReader : ICsvReader
    {
      private static readonly string[] columnNames;
      private static readonly PropertyInfo[] daoProperties; 

      private static CsvReader()
      {
          columnNames=MetaData.GetColumnNames();
          if (columnNames.Length  == 0)
          {
              throw new Exception(MethodBase.GetCurrentMethod.Name+
              ": xml metadata must be parsed first");
          }
          PropertyInfo[] properties=typeof(IDataSpecs).GetProperties();
          if (properties.Length != columnNames.Length)
          {
              throw new Exception(MethodBase.GetCurrentMethod.Name+
              ": data object properties length "+properties.Length+
              " does not match specified length "+columnNames.Length);
          }
          for (byte colIdx=0; colIdx < columnNames.Length; colIdx++)
          {
              float maxPct=0f;
              short idxOfMax=-1;
              for (byte idx=0; idx < properties.Count; idx++)
              {
                string lcs=Utilities.LongestCommonSubstring(properties[idx].Name, columnNames[colIdx]);
                float matchPct=lcs.Length/(float)columnNames[colIdx].Length;
                if (matchPct > maxPct)
                {
                    maxPct=matchPct;
                    idxOfMax=idx;
                } 
              }
              if (idxOfMax == -1)
              {
                  throw new Exception(MethodBase.GetCurrentMethod.Name+": name " + columnNames[colIdx] +
                   " does not match data object property");
              }
              // put property that matches name in same order as column names
              PropertyInfo temp=properties[colIdx];
              properties[colIdx]=properties[idxOfMax];
              properties[idxOfMax]=temp;
          }
          // check that types match
          for (byte i=0; i < columnNames.Length; i++)
          {
              // compare types in data object with types specified
          if (!properties[i].PropertyType.IsAssignableFrom(MetaData.getColumnType(columnNames[i])))
          {
             // throw exception if types are not assignable 
             throw new Exception(MethodBase.GetCurrentMethod.Name+": type "+
             properties[i].PropertyType.FullName+ " does not match specified type "+
             MetaData.getColumnType(columnNames[i]).FullName);
          }
          }
          // set ordered properties
          daoProperties=properties;
      }
        public string GetNextLatestCsv()
        {
            Regex rgx=new Regex(@"(\w+)\.(\d{4}\.\d{2}\.\d{2})",
                              RegexOptions.IgnoreCase|RegexOptions.Compiled);
            string dataDir=ConfigurationManager.AppSettings["data-dir"];
            
             var maxes= (from file in Directory.GetFiles(dataDir,"*.csv") // iterate over csv files
             let match=rgx.Match(file) // get matches
             where (match.Success)
             select new {f=file, coname=match.Captures[0].Value,
             date=DateTime.ParseExact(match.Captures[1].Value,"yyyy.MM.dd", // convert date file part into an actual date
             System.Globalization.CultureInfo.InvariantCulture)}). 
             GroupBy(o=>o.coname,(key, os) => os.OrderByDescending(o => o.date).First()).// group by company name order by date descending
                       AsParallel().ToArray();                                           // so bigest date is on top for each company
             foreach (var maxFile in maxes)
             {
                 // return file name that has max date
                 yield return maxFile.f;
             }
             // no more filenames to return
             return null;
        }
          public IDataSpecs? ReadLine(string filename)
        {
            // validate input
            if (!File.Exists(filename))
            {
                throw new Exception(MethodBase.GetCurrentMethod.Name+": "+filename+
                " does not exist in file system");
            }
            string line;
            // open file name
            using (StreamReader reader=new StreamReader(filename,System.Text.Encoding.ASCII))
            {
            while ((line=reader.ReadLine()) != null)
            {
               string[] results=line.Split(',');
               // populate data object
               DataSpec spec = new DataSpec();
               for (byte i=0; i < daoProperties.Length; i++)
               {
                   daoProperties[i].SetValue(spec,Convert.ChangeType(results[i],
                   MetaData.getColumnType(columnNames[i])));
               }
               yield return spec; 
            }
            }
            // no object to return
            return null;
}
         public string GetCompanyName(string filename)
         {
             // validate input
            if (!File.Exists(filename))
            {
                throw new Exception(MethodBase.GetCurrentMethod.Name+": "+filename+
                " does not exist in file system");
            }
            return Regex.Match(filename, @"(\w+)\.\d{4}\.\d{2}\.\d{2}",
                        RegexOptions.IgnoreCase|RegexOptions.Compiled).Value;
         }
    }
}
