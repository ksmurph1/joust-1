using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
namespace DAO
{
    public class CompanyInventory : IInventory
    {
      private List<IDataSpecs> rows= new List<IDataSpecs>();
      private readonly string title;   
    

      public IDataSpecs GetRow()
      {
         
      }
      public Type GetItem(byte idx)
      {
          
      }
 
    }
}