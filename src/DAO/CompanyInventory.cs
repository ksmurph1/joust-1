using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using System;
using DL;
namespace DAO
{
    public class CompanyInventory : IInventory
    {
      private List<IDataSpecs> rows= new List<IDataSpecs>();
      private readonly string title;
      private readonly ICsvReader reader;   
      public string CompanyName {get{ return title;} private set;}
      public CompanyInventory<T>(out ValueReturnObj<T> statusObj)
      {
          ValueReturnObj<T> statusObj;
          reader=new CsvReader(FileSelector.GetNextLatestCsv(),out statusObj);
          if (statusObj == null)
          {
              // no exceptions- we can continue
              title=reader.GetCompanyName();

              ValueReturnObj<Nullable<IDataSpecs>>[] statusObjs=reader.ReadLines();
             
             if (statusObjs.All(so=>so.Value != null))
             {
              // if now exception populate rows of inventory
              foreach (ValueReturnObj<Nullable<IDataSpecs>> statusObj in  statusObjs)
              {
                  rows.Add(statusObj.Value.Value);
              }
             }
             else
             {
                 statusObj=new ValueReturnObj<T> 
                 {
                     Exception=MethodBase.GetCurrentMethod.Name+": "+statusObjs.First(so=>so.Exception!=null).Exception;
                 };
             }

          }
      }
      public IDataSpecs GetRow<T>(Func<T,bool> predicate)
      {
         
      }
 
    }
}