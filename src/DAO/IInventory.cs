using System;
using System.Collections.Generic;
using Util;
using DAL;
namespace DAO
{
    public interface IInventory
    {
        string CompanyName {get;}
        void FillInventory(out ValueReturnObj<object> statusObj);
        IList<ValueReturnObj<KeyValuePair<T,IDataSpecs>>> ApplyOperation<T>(Func<IDataSpecs,T> operation,
         Func<IDataSpecs,bool> predicate);
    }
}