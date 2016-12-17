using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System;
using DAL;
using Util;
namespace DL
{
    public struct CsvReader : ICsvReader
    {
        private static readonly string[] columnNames;
        private static readonly PropertyInfo[] daoProperties;
        private readonly FileInfo thisFile;
        static CsvReader()
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            columnNames = MetaData.GetColumnNames();
            if (columnNames.Length == 0)
            {
                throw new Exception(methodName + ": xml metadata must be parsed first");
            }
            PropertyInfo[] properties = typeof(IDataSpecs).GetProperties();
            if (properties.Length != columnNames.Length)
            {
                throw new Exception(methodName +
                ": data object properties length " + properties.Length +
                " does not match specified length " + columnNames.Length);
            }
            for (byte colIdx = 0; colIdx < columnNames.Length; colIdx++)
            {
                float maxPct = 0f;
                short idxOfMax = -1;
                for (byte idx = 0; idx < properties.Length; idx++)
                {
                    string lcs = Utilities.LongestCommonSubstring(properties[idx].Name, columnNames[colIdx]);
                    float matchPct = lcs.Length / (float)columnNames[colIdx].Length;
                    if (matchPct > maxPct)
                    {
                        maxPct = matchPct;
                        idxOfMax = idx;
                    }
                }
                if (idxOfMax == -1)
                {
                    throw new Exception(methodName + ": name " + columnNames[colIdx] +
                     " does not match data object property");
                }
                // put property that matches name in same order as column names
                PropertyInfo temp = properties[colIdx];
                properties[colIdx] = properties[idxOfMax];
                properties[idxOfMax] = temp;
            }
            // check that types match
            for (byte i = 0; i < columnNames.Length; i++)
            {
                // compare types in data object with types specified
                if (!properties[i].PropertyType.IsAssignableFrom(MetaData.getColumnType(columnNames[i])))
                {
                    // throw exception if types are not assignable 
                    throw new Exception(methodName + ": type " +
                    properties[i].PropertyType.FullName + " does not match specified type " +
                    MetaData.getColumnType(columnNames[i]).FullName);
                }

            }
            // set ordered properties
            daoProperties = properties;
        }

        public CsvReader(FileInfo file, out ValueReturnObj<T> statusObj)
        {
            ValueReturnObj<T> status = new ValueReturnObj<T>();
            thisFile = null;
            // validate input
            if (!file.Exists)
            {
                status.Exception = new Exception(MethodBase.GetCurrentMethod.Name + ":" + file.FullName +
                " does not exist in file system");
            }
            else
            {
                // assigning class member variable
                thisFile = file;
            }
            statusObj = status;
        }


        public ValueReturnObj<Nullable<IDataSpecs>>[] ReadLines()
        {
            List<ValueReturnObj<Nullable<IDataSpecs>>> statusObjs = new List<ValueReturnObj<Nullable<IDataSpecs>>>();

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
                        for (byte i = 0; i < daoProperties.Length; i++)
                        {
                            daoProperties[i].SetValue(spec, Convert.ChangeType(results[i],
                            MetaData.getColumnType(columnNames[i])));
                        }
                        ValueReturnObj<Nullable<IDataSpecs>> statusObj = new ValueReturnObj<Nullable<IDataSpecs>>
                        {
                            Value = spec
                        };
                        statusObjs.Add(statusObj);
                    }
                }
            }
            catch (Exception e)
            {
                statusObjs.Add(new ValueReturnObj<Nullable<IDataSpecs>>
                {
                    Exception = new Exception(MethodBase.GetCurrentMethod().Name + ":" + e.Message)
                });
            }
            return statusObjs.ToArray();
        }
        public IValueReturnObj<string> GetCompanyName()
        {
            IValueReturnObj<string> statusObj;
            try
            {
                statusObj = new ValueReturnObj<string>
                {
                    Value = Regex.Match(thisFile.FullName, @"(\w+)\.\d{4}\.\d{2}\.\d{2}",
                            RegexOptions.IgnoreCase | RegexOptions.Compiled).Value
                };
            }
            catch (Exception e)
            {
                statusObj = new ValueReturnObj<string>
                {
                    Exception = new Exception(MethodBase.GetCurrentMethod().Name + ": " + e.Message)
                };
            }
            return statusObj;
        }
    }
}
