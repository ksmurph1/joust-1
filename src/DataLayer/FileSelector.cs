using Util;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DataLayer
{

    public struct FileSelector
    {
        static FileSelector()
        {
            Regex rgx = new Regex(@"(\w+,?\s*\w*)\.(\d{4}\.\d{2}\.\d{2})",
                                        RegexOptions.IgnoreCase | RegexOptions.Compiled);
            ConcurrentBag<FileInfo> paths = new ConcurrentBag<FileInfo>();

            try
            {

                string dataDir = ConfigurationManager.AppSettings["data-dir"];

                var maxes = (from file in Directory.GetFiles(dataDir, "*.csv") // iterate over csv files
                             let match = rgx.Match(file) // get matches
                             where (match.Success)
                             select new
                             {
                                 f = file,
                                 coname = match.Groups[1].Value,
                                 date = DateTime.ParseExact(match.Groups[2].Value, "yyyy.MM.dd", // convert date file part into an actual date
                             System.Globalization.CultureInfo.InvariantCulture)
                             }).
                             //have to do it this way in order to represent stable sort in parallel
                GroupBy(o => o.coname, (key, os) => os.AsParallel().Select((e,idx)=> new { elem =e, idx = idx }).OrderByDescending(ao => ao.elem.date).
                ThenByDescending(ao=>ao.idx).Select(ao=>ao.elem).First())// group by company name order by date descending
                          .ToArray();

                Parallel.ForEach(maxes, ao =>
                 {
                    // return file name that has max date
                    paths.Add(new FileInfo(ao.f));
                 });
            }
            catch (Exception e)
            {
                throw new Exception("FileSelector:" + e.Message);

            }

            maxPaths = paths.ToArray();
        }
        private static FileInfo[] maxPaths;
        private static int index = 0;

        public static bool IsDone { get; private set; }

        public static ValueReturnObj<FileInfo> GetNextLatestCsv()
        {
            ValueReturnObj<FileInfo> statusObj = new ValueReturnObj<FileInfo>();
            if (index == maxPaths.Length)
            {
                // invalidate index
                index = -1;
                // set done flag
                FileSelector.IsDone = true;
            }
            else if (index != -1)
            {
                statusObj.Value = maxPaths[index++];

            }
            // return status object
            return statusObj;
        }
    }
}