using Util;
using System;
using System.Text.RegularExpressions;
using System.Linq;
namespace DL
{

public static struct FileSelector
{
   static FileSelector()
   {
  Regex rgx=new Regex(@"(\w+)\.(\d{4}\.\d{2}\.\d{2})",
                              RegexOptions.IgnoreCase|RegexOptions.Compiled);
                              List <FileInfo> paths=new List<FileInfo>();

try
{
    
            string dataDir=ConfigurationManager.AppSettings["data-dir"];
            
             var maxes= (from file in Directory.GetFiles(dataDir,"*.csv") // iterate over csv files
             let match=rgx.Match(file) // get matches
             where (match.Success)
             select new {f=file, coname=match.Captures[0].Value,
             date=DateTime.ParseExact(match.Captures[1].Value,"yyyy.MM.dd", // convert date file part into an actual date
             System.Globalization.CultureInfo.InvariantCulture)}). 
             GroupBy(o=>o.coname,(key, os) => os.OrderByDescending(o => o.date).First()).// group by company name order by date descending
                       AsParallel().ToArray();
                   
             foreach (var maxFile in maxes)
             {
                 // return file name that has max date
                    paths.Add(new FileInfo(maxFile.f));
             }
                            }
                                    catch (Exception e)
                              {
                                e.Message=MethodBase.GetCurrentMethod.Name+":"+e.Message;
                                throw e;
                              }
                            
        maxPaths=paths.ToArray();
   }
   private static FileInfo[] maxPaths;
   private static int index=0;
  public static ValueReturnObj<FileInfo> GetNextLatestCsv()
        {
            ValueReturnObj<FileInfo> statusObj;
            if (index == paths.Count)
            {
                // invalidate index
                index=-1;
            }
            else if (index != -1)
            {
                statusObj=new ValueReturnObj<FileInfo>
                {
                    Value=maxPaths[index++]
                }
            }           
            // null is returned if no more elements
            return statusObj;
            }
}