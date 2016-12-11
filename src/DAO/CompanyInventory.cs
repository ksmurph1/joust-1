using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using System;

namespace DAO
{
    public class CompanyInventory : IInventory
    {
      private List<IDataSpecs> rows= new List<IDataSpecs>();
      private readonly string title;
      private readonly ICsvReader reader=new CsvReader();   
    
      public CompanyInventory()
      {
          FileInfo file=reader.GetNextLatestCsv();
          if (file == null)
          {
            
          } 
      }
      public IDataSpecs GetRow()
      {
         
      }
      public Type GetItem(byte idx)
      {
          
      }
 
    }
}