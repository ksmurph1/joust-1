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
      private static readonly PropertyInfo[] daoProperties; 

      private static CompanyInventory()
      {
          columnNames=MetaData.GetColumnNames();
          if (columnNames.Length  == 0)
          {
              throw new Exception(System.Reflection.MethodBase.GetCurrentMethod.Name+
              ": xml metadata must be parsed first");
          }
          PropertyInfo[] properties=typeof(IDataSpecs).GetProperties();
          if (properties.Length != columnNames.Length)
          {
              throw new Exception(System.Reflection.MethodBase.GetCurrentMethod.Name+
              ": data object properties length "+properties.Length+
              " does not match specified length "+columnNames.Length);
          }
          for (byte colIdx=0; colIdx < columnNames.Length; colIdx++)
          {
              float maxPct=0f;
              short idxOfMax=-1;
              for (byte idx=0; idx < properties.Count; idx++)
              {
                string lcs=LongestCommonSubstring(properties[idx].Name, columnNames[colIdx]);
                float matchPct=lcs.Length/(float)columnNames[colIdx].Length;
                if (matchPct > maxPct)
                {
                    maxPct=matchPct;
                    idxOfMax=idx;
                } 
              }
              if (idxOfMax == -1)
              {
                  throw new Exception(System.Reflection.MethodBase.GetCurrentMethod.Name+
                   ": name " + columnNames[colIdx] + " does not match data object property");
              }
              // put property that matches name in same order as column names
              PropertyInfo temp=properties[colIdx];
              properties[colIdx]=properties[idxOfMax];
              properties[idxOfMax]=temp;
          }
          // set ordered properties
          daoProperties=properties;
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