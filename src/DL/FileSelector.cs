using Util;
using System;
using System.Text.RegularExpressions;
using System.Linq;
namespace DL
{

public struct FileSelector
{

  public static System.Generic.Collections.IEnumerable<ValueReturnObj<FileInfo>> GetNextLatestCsv()
        {
            
            ValueReturnObj<FileInfo> statusObj;
                
            Regex rgx=new Regex(@"(\w+)\.(\d{4}\.\d{2}\.\d{2})",
                              RegexOptions.IgnoreCase|RegexOptions.Compiled);
                              dynamic maxes;
                              try
                              {
                          
            string dataDir=ConfigurationManager.AppSettings["data-dir"];
            
             maxes= (from file in Directory.GetFiles(dataDir,"*.csv") // iterate over csv files
             let match=rgx.Match(file) // get matches
             where (match.Success)
             select new {f=file, coname=match.Captures[0].Value,
             date=DateTime.ParseExact(match.Captures[1].Value,"yyyy.MM.dd", // convert date file part into an actual date
             System.Globalization.CultureInfo.InvariantCulture)}). 
             GroupBy(o=>o.coname,(key, os) => os.OrderByDescending(o => o.date).First()).// group by company name order by date descending
                       AsParallel().ToArray();
                              }
                                    catch (Exception e)
                              {
                                e.Message=MethodBase.GetCurrentMethod.Name+":"+e.Message;
                            statusObj=new ValueReturnObj<FileInfo>
                 {
                     Exception=e
                 };
                 yield break;
                              }                                           // so bigest date is on top for each company
             foreach (var maxFile in maxes)
             {
                 // return file name that has max date
                 statusObj=new ValueReturnObj<FileInfo>
                 {
                    Value=new FileInfo(maxFile.f)
                 };
                 yield return statusObj;
             }
                 
            }
        }
}