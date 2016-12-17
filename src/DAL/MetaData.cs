using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System;
namespace DAL
{
    public struct MetaData
    {
        private static ConcurrentBag<KeyValuePair<byte,string>> columnNames=
            new ConcurrentBag<KeyValuePair<byte, string>>();
        private static ConcurrentBag<KeyValuePair<byte,Type>> columnTypes=
            new ConcurrentBag<KeyValuePair<byte, Type>>();

        internal static void addColumn(byte index, string name, Type colType)
        {
            columnNames.Add(new KeyValuePair<byte,string>(index, name));
            columnTypes.Add(new KeyValuePair<byte,Type>(index, colType));
        }

        public static string[] getColumnNames()
        {
            string[] names= new string[columnNames.Count];
            KeyValuePair<byte,string> result;
            while (!columnNames.IsEmpty)
            {
                if (columnNames.TryTake(out result))
                {
                    names[result.Key]=result.Value;
                }
            }
            return names;
        }

        public static Type getColumnType(string colName)
        {
            Type result = null;
             columnTypes.Zip(columnNames, (kpt, n) =>
             {
                if (n.Value == colName && n.Key == kpt.Key)
                 {
                     return kpt.Value;
                 }
                 else
                 {
                     return null;
                 }
             }).FirstOrDefault(t=>t!=null);
            return result;
        }
    }
}