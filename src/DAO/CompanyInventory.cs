using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Linq;
using System;
namespace DAO
{
    public class CompanyInventory : IInventory
    {
      private List<DataRow> rows= new List<DataRow>();  
      private static readonly string[] columnNames;
      public string[] Columns { get { return columnNames;}}
      private static CompanyInventory()
      {
          columnNames=MetaData.GetColumnNames();
          if (columnNames.Length  == 0)
          {
              throw new Exception(System.Reflection.MethodBase.GetCurrentMethod.Name+
              ": xml metadata must be parsed first");
          }
          List<PropertyInfo> properties=typeof(IDataSpecs).GetProperties().ToList<PropertyInfo>();
          foreach (string name in columnNames)
          {
              float maxPct=0f;
              short idxOfMax=-1;
              for (byte idx=0; idx < properties.Count; idx++)
              {
                string lcs=LongestCommonSubstring(properties[idx].Name, name);
                float matchPct=lcs.Length/(float)name.Length;
                if (matchPct > maxPct)
                {
                    maxPct=matchPct;
                    idxOfMax=idx;
                } 
              }
              if (idxOfMax == -1)
              {
                  throw new Exception("name " + name + " does not match data object property");
              }
              else if (properties[idxOfMax].PropertyType != MetaData.GetColumnType(name))
              {
                  throw new Exception("type mismatch between data object and what is )
              }
          }
      }
      public void AddRow(string[] data)
      {

      }
      public Type GetItem(byte idx)
      {
          
      }
      private static string LongestCommonSubstring(string stringA, string stringB)
{
    var allSubstrings = new List<string>();
    for(int substringLength = stringA.Length -1; substringLength >0; substringLength--)
    {
        for(int offset = 0; (substringLength + offset) < stringA.Length; offset++)
        {
            string currentSubstring = stringA.Substring(offset,substringLength);
            if (!System.String.IsNullOrWhiteSpace(currentSubstring) && !allSubstrings.Contains(currentSubstring))
            {
                allSubstrings.Add(currentSubstring);
            }
        }
    }

    return allSubstrings.OrderBy(subStr => subStr).ThenByDescending(subStr => subStr.Length).
           Where(subStr => stringB.Contains(subStr)).DefaultIfEmpty(s=>String.Empty).First();
}
    }
}