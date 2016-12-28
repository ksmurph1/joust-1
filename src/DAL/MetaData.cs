using System.Linq;
using System;
using System.Collections.Concurrent;

namespace Descriptor
{
    public struct MetaData
    {
        private static ConcurrentDictionary<string,Type> columnDescs=
            new ConcurrentDictionary<string, Type>(new Util.CaseInsComparer());
        static MetaData()
        {
            // process meta data
            FileMetaParser.ParseMetaData();
        }
        internal static void Add(string name, Type colType)
        {
            columnDescs[name]=colType;
        }

        public static string[] GetColumnNames()
        {
            return columnDescs.Keys.ToArray();
        }

        public static Type GetColumnType(string colName)
        {
            // get value that matches key
            Type value;
            columnDescs.TryGetValue(colName, out value);
            return value;
        }
    }
}