using System.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Descriptor
{
    public struct MetaData
    {
        private static ConcurrentDictionary<KeyValuePair<byte,string>,Type> columnDescs=
            new ConcurrentDictionary<KeyValuePair<byte,string>, Type>(new Util.CaseInsComparer());
        private static volatile bool _completed = false;
        public static bool IsCompleted { get { return _completed; } internal set { _completed = value; } }

        static MetaData()
        {
            // process meta data
            FileMetaParser.ParseMetaData();
        }
        internal static void Add(byte index,string name, Type colType)
        {
            columnDescs.TryAdd(new KeyValuePair<byte, string>(index,name),colType);
        }

        public static string[] GetColumnNames()
        {
            return columnDescs.Keys.OrderBy(kp=>kp.Key).Select(kp=>kp.Value).ToArray();
        }

        public static Type GetColumnType(string colName)
        {
            var matchingStr=columnDescs.AsParallel().First(
                                            kpv => String.Compare(colName, kpv.Key.Value, true) == 0);
            // get value that matches key
            return matchingStr.Value;
        }
    }
}