using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using System.IO;
using System;
using DataLayer;
using DataObject;
using Util;
namespace Supplier
{
    public class CompanyInventory : IInventory
    {
        private List<IDataSpecs> rows = new List<IDataSpecs>();
        private string title;
        public string CompanyName { get { return title; } }
        private MethodInfo conditional;
        private CompanyInventory()
        { }

        public static IValueReturnObj<IInventory> FillInventory()
        {
            IValueReturnObj<IInventory> statusObj = new ValueReturnObj<IInventory>();
            string methodName = "FillInventory()";
  
            try
            {
                IValueReturnObj<FileInfo> csvStatusObj;

                ICsvReader reader = new CsvReader(out csvStatusObj);
                   if (!reader.IsDone)   // continue if not done
                    {
                if (csvStatusObj.HasVal)
                {
                    IValueReturnObj<IDataSpecs[]> rStatusObj;
                 
                        rStatusObj = reader.ReadLines();

                        if (rStatusObj.HasVal)
                        {
                            // construct a new instance for storing rows
                            CompanyInventory invHolder = new CompanyInventory();

                            // if no exception populate rows of inventory
                            invHolder.rows.AddRange(rStatusObj.Value);

                            // no exceptions- we can continue
                            IValueReturnObj<string> title = reader.GetCompanyName();

                            // get the company name
                            if (!title.HasVal)
                            {
                                invHolder.title = String.Empty;
                            }
                            else
                            {
                                invHolder.title = title.Value;
                            }
                            statusObj.Value = invHolder;
                        }
                        else
                        {
                            statusObj.Exception = new Exception(methodName + ": " + rStatusObj.Exception?.Message);
                        }
                }
                else
                {
                    statusObj.Exception = new Exception(methodName + ": Something wrong with csv reader :" + csvStatusObj.Exception.Message);
                }
                    }
            }
            catch (Exception ex)
            {
                statusObj.Exception = new Exception(methodName + ": Something went wrong with filling inventory :"+ex);
            }
            return statusObj;
        }
        public IValueReturnObj<IDataSpecs> GetRow(Guid id)
        {
            IValueReturnObj<IDataSpecs> statusObj = new ValueReturnObj<IDataSpecs>();
            try
            {
                statusObj.Value= rows.Find(d => d.ID.Equals(id));
               
            }
            catch (Exception ex)
            {
                statusObj.Exception = new Exception("GetRow(" + id+ "): " + ex.Message);
            }
            return statusObj;
        }
        public void SetCriteria<T>(Func<T, bool> criteria)
        {
            this.conditional = criteria.GetMethodInfo();
        }

        public IList<IValueReturnObj<KeyValuePair<T, IDataSpecs>>> ApplyOperation<T>(Func<IDataSpecs, T> operation)
        {
            IValueReturnObj<KeyValuePair<T, IDataSpecs>> statusObj = new ValueReturnObj<KeyValuePair<T, IDataSpecs>>();
           ReadOnlyCollection<IValueReturnObj<KeyValuePair<T, IDataSpecs>>> result=
            new ReadOnlyCollection<IValueReturnObj<KeyValuePair<T, IDataSpecs>>>(
                new IValueReturnObj<KeyValuePair<T, IDataSpecs>>[] { statusObj });
                try
                {
                    IEnumerable<IDataSpecs> collection;

                    // set conditional for rows to select and return new collection
                    if (conditional == null)
                    {
                        collection = rows;
                    }
                    else
                    {
                        collection = rows.Where(d => (bool)conditional.Invoke(this, new object[] { d }));
                    }
                    result=new ReadOnlyCollection<IValueReturnObj<KeyValuePair<T, IDataSpecs>>>(collection.AsParallel().Select(
                        d => new ValueReturnObj<KeyValuePair<T, IDataSpecs>>
                        {
                 // apply operation on each data object and store as key, value
                 Value = new KeyValuePair<T, IDataSpecs>(operation(d), d)
                        }).Cast< IValueReturnObj<KeyValuePair<T, IDataSpecs>>>().ToArray());
                }
                catch (Exception e)
                {

                    statusObj.Exception = new Exception("ApplyOperation<T>("+operation + "): " + e.Message);
                }
            return result;
        }

    }
}