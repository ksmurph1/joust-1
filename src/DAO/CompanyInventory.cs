using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using System;
using DL;
using DAL;
namespace DAO
{
    public class CompanyInventory : IInventory
    {
        private List<IDataSpecs> rows = new List<IDataSpecs>();
        private string title;
        private readonly ICsvReader reader;
        private bool invFilled = false;
        private readonly FileInfo sourceFile;
        public string CompanyName { get { return title; } private set; }
        private CompanyInventory(FileInfo sourceFile)
        {
            this.sourceFile = sourceFile;
        }

        public static ValueReturnObj<IInventory> CreateInstance()
        {
            ValueReturnObj<IInventory> result = new ValueReturnObj<IInventory>();
            ValueReturnObj<FileInfo> statusObj = FileSelector.GetNextLatestCsv();
            if (statusObj.Value != null)
            {
                result.Value = new CompanyInventory(statusObj.Value);
            }
            else
            {
                result.Exception = MethodBase.GetCurrentMethod().Name + ": No more Inventory to process" + statusObj.Exception;
            }
            return result;
        }

        public void FillInventory(out ValueReturnObj<object> statusObj)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            ValueReturnObj<object> csvStatusObj = null;
            statusObj = new ValueReturnObj<object>();
            ICsvReader reader = new CsvReader(sourceFile, out csvStatusObj);
            if (csvStatusObj.Exception == null)
            {
                // no exceptions- we can continue
                title = reader.GetCompanyName();

                ValueReturnObj<Nullable<IDataSpecs>>[] statusObjs = reader.ReadLines();

                if (statusObjs.All(so => so.Value != null))
                {
                    // if now exception populate rows of inventory
                    foreach (ValueReturnObj<Nullable<IDataSpecs>> dstatusObj in statusObjs)
                    {
                        rows.Add(dstatusObj.Value.Value);
                    }
                    // set flag
                    invFilled = true;
                }
                else
                {
                    statusObj.Exception = new Exception(methodName + ": " + statusObjs.First(so => so.Exception != null).Exception.Message);
                }
            }
            else
            {
                statusObj.Exception = new Exception(methodName + ": Something wrong with csv reader :" + csvStatusObj.Exception.Message);
            }
        }
        public IDataSpecs[] GetRows<T>(Func<T, bool> predicate)
        {

        }

        public bool IsInventoryFilled()
        {
            return invFilled;
        }

        public IList<ValueReturnObj<KeyValuePair<T, IDataSpecs>>> ApplyOperation<T>(Func<IDataSpecs, T> operation,
                                                            Func<IDataSpecs, bool> predicate)
        {
            ValueReturnObj<KeyValuePair<T, IDataSpecs>> statusObj = new ValueReturnObj<KeyValuePair<T, IDataSpecs>>();
            if (invFilled)
            {
                try
                {

                    return Array.AsReadOnly(rows.Where(predicate).Select(
                        d => new ValueReturnObj<KeyValuePair<T, IDataSpecs>>
                        {
                 // apply operation on each data object and store as key, value
                 Value = new KeyValuePair<T, IDataSpecs>(operation(d), d)
                        }).AsParallel().ToArray());
                }
                catch (Exception e)
                {

                    statusObj.Exception = new Exception(MethodBase.GetCurrentMethod().Name + ": " + e.Message);
                }
            }
            return statusObj;
        }

    }
}