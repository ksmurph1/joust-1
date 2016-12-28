using System;
using System.Collections.Generic;
using Util;
using DataObject;
namespace Supplier
{
    public interface IInventory
    {
        string CompanyName { get;}
        void SetCriteria<T>(Func<T, bool> criteria);
        IList<IValueReturnObj<KeyValuePair<T,IDataSpecs>>> ApplyOperation<T>(Func<IDataSpecs,T> op);
        IValueReturnObj<IDataSpecs> GetRow(Guid id);
    }
}