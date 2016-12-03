namespace DAL
{
    public static class MetaData
    {
        private static List<string> columnNames;
        private static List<Type> columnTypes;

        public static void addColumn(string name, Type colType)
        {
            columnNames.Add(name);
            columnTypes.Add(colType);
        }

        public static string[] getColumnNames()
        {
            return columnNames.ToArray();
        }

        public static Type getColumnType(string colName)
        {
            Type result;
            int index=columnNames.FindIndex(s=>s==colName);
            if (index >= 0)
               result=columnTypes[index];
            else
               result=typeof(Util.Null);
            return result;
        }
    }
}