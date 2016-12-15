using System;
using System.Collections.Generic;
using Util;
namespace DAO
{
    public interface IInventory
    {
        string CompanyName {get;}
        void FillInventory(out ValueReturnObj<object>);
        IList<ValueReturnObj<KeyValuePair<T,IDataSpecs>>> ApplyOperation<T>(Func<IDataSpecs,T>, Func<IDataSpecs,bool>);
    }
}