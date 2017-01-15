using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System;
using Descriptor;
using Util;
using DataObject;
using System.Threading.Tasks;

namespace DataLayer
{
    public struct CsvReader : ICsvReader
    {
        private static readonly string[] columnNames;
        private static readonly PropertyInfo[] daoProperties;
        private readonly FileInfo thisFile;

        public bool IsDone { get; private set; }

        private struct CaseInsComparer : IEqualityComparer<string>
        {
            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }

            public bool Equals(string a, string b)
            {
                bool status = false;
                if (String.Compare(a, b, true) == 0)
                {
                    status = true;
                }
                return status;
            }
        };
        static CsvReader()
        {
            string methodName = "static CsvReader";
            // wait for meta data collection to finish
            while (!MetaData.IsCompleted)
            {
                if (MetaData.IsCompleted)
                {
                    break;
                }
            }
            columnNames = MetaData.GetColumnNames();
           

            if (columnNames.Length == 0)
            {
                throw new Exception(methodName + ": xml metadata must be parsed first");
            }
            PropertyInfo[] properties = typeof(IDataSpecs).GetProperties();
           
            // make sure columnNames and properties match
//            columnNames=columnNames.AsParallel().Intersect(properties.AsParallel().Select(pi=>pi.Name),
  //                             new CaseInsComparer()).ToArray();
           
            // make sure property order matches column names and names match
            ILookup<string,PropertyInfo> lookup=properties.ToLookup(pi => pi.Name, new CaseInsComparer());
            properties=columnNames.SelectMany(name => lookup[name]).ToArray();

            if (properties.Length != columnNames.Length)
            {
                throw new Exception(methodName +
                ": data object properties length " + properties.Length +
                " does not match specified length " + columnNames.Length);
            }
            //AsParallel blocks forever here for some reason, but not an issue
            if (properties.TakeWhile((pi, idx) => pi.PropertyType.IsAssignableFrom(
                                               MetaData.GetColumnType(columnNames[idx]))).Count() != properties.Length)
            {
                // throw exception if types are not assignable 
                throw new Exception(methodName + ": types " +
                String.Join(", ",properties.Select(p=>p.PropertyType.FullName)) + " does not match specified types " +
                String.Join(", ",columnNames.Select(s=>MetaData.GetColumnType(s).FullName)));
            }
            // if we made it this far then set properties
            daoProperties = properties;
        }
        /*private static bool CheckDataTypesMatch()
        {
            bool status = false;

            // check that types match
            Parallel.For(0, columnNames.Length, (idx,pls) =>
            {
                // compare types in data object with types specified
                if (daoProperties[idx].PropertyType.IsAssignableFrom(MetaData.GetColumnType(columnNames[idx])))
                {
                    status = true;
                    pls.Break();          
                }

            });
            return status;
        }*/
        public CsvReader(out IValueReturnObj<FileInfo> statusObj)
        {
            thisFile = null;
            statusObj = FileSelector.GetNextLatestCsv();
            if (!FileSelector.IsDone)
            {
                IsDone = false;
                if (statusObj.HasVal)
                {
                    // assigning class member variable
                    thisFile = statusObj.Value;
                }
                else
                {
                    statusObj.Exception = new Exception("CsvReader(): Problem getting next file to process: " +
                                         statusObj.Exception);
                }
            }
            else
            {
                IsDone = true;
            }
        }


        public IValueReturnObj<IDataSpecs[]> ReadLines()
        {
            List<IDataSpecs> dataObjs = new List<IDataSpecs>();
            ValueReturnObj < IDataSpecs[] > statusObj=new ValueReturnObj<IDataSpecs[]>();
            try
            {
                // open file name
                using (StreamReader reader = new StreamReader(thisFile.OpenRead(), System.Text.Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] results = line.Split(',');
                        // populate data object
                        IDataSpecs spec = new DataSpec();
                        Parallel.For(0, daoProperties.Length, (idx)=>
                         {
                         daoProperties[idx].SetValue(spec, System.ComponentModel.TypeDescriptor.GetConverter(
                         MetaData.GetColumnType(columnNames[idx])).ConvertFromString(results[idx]));  
                         });
                        dataObjs.Add(spec);
                    }
                }
                statusObj.Value = dataObjs.ToArray();
            }
            catch (Exception e)
            {
                statusObj.Exception = new Exception("ReadLines():" + e.Message);
            }
            return statusObj;
        }
        public IValueReturnObj<string> GetCompanyName()
        {
            IValueReturnObj<string> statusObj;
            try
            {
                statusObj = new ValueReturnObj<string>
                {
                    Value = Regex.Match(thisFile.FullName, @"(\w+,?\s*\w*)\.\d{4}\.\d{2}\.\d{2}",
                            RegexOptions.IgnoreCase | RegexOptions.Compiled).Groups[1].Value
                };
            }
            catch (Exception e)
            {
                statusObj = new ValueReturnObj<string>
                {
                    Exception = new Exception("GetCompanyName(): " + e.Message)
                };
            }
            return statusObj;
        }
    }
}
