using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
namespace DAL
{
    public static class MetaData
    {
        private static ConcurrentBag<KeyValuePair<byte,string>> columnNames;
        private static ConcurrentBag<KeyValuePair<byte,Type>> columnTypes;

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
            int index=columnNames.First(kp=>kp.Value == colName).Select(kp=>kp.Key).DefaultIfEmpty(-1);
            Type result=columnTypes.Where(kp=>kp.Key == index).Select(kp=>kp.Value).DefaultIfEmpty(typeof(Util.Null));
            return result;
        }
    }
}