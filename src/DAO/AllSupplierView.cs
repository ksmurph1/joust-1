using Util;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Supplier
{
    public class AllSupplierView
    {
        private readonly ConcurrentStack<IInventory> suppliers = new ConcurrentStack<IInventory>();
        private bool exceptional = false;
        private IList<IInventory> readOnlyCollection;
        public AllSupplierView(out IValueReturnObj<object> statusObj)
        {
            bool status = true;
            statusObj = new ValueReturnObj<object>
            {
                HasVal = true
            };
            System.Threading.CancellationTokenSource cToken = new System.Threading.CancellationTokenSource();
            Stack<Task> tasks = new Stack<Task>();
            while (status)
            {
                try
                {
                    // fill inventory of company
                    tasks.Push(Task.Factory.StartNew(() =>
                     CompanyInventory.FillInventory(), cToken.Token).ContinueWith(t =>
                      {
                         if (t.Result.HasVal)
                         {
                        // if no errors from filling
                        suppliers.Push(t.Result.Value);
                         }
                         else
                         {
                             if (t.Result.Exception != null)
                             {
                              // exception occurred
                                exceptional = true;
                             }
                        // otherwise set loop condition to fail
                        cToken.Cancel(true);
                         }
                     }, cToken.Token));
                }
                catch (AggregateException ex)
                {
                    status = false; // terminate loop
                    if (exceptional)
                    {
                        // store exception if not empty
                        statusObj.Exception = ex.GetBaseException().InnerException;
                    }
                }
            }
        }

        public IValueReturnObj<IList<IInventory>> Suppliers
        {
            get
            {
                IValueReturnObj<IList<IInventory>> statusObj = new ValueReturnObj<IList<IInventory>>(); 
                if (!exceptional && readOnlyCollection == null)
                {
                    // if read only collection does not exist, create it once
                    readOnlyCollection= new
                     System.Collections.ObjectModel.ReadOnlyCollection<IInventory>(suppliers.ToArray());
                    statusObj.Value = readOnlyCollection;
                }
                else if (exceptional)
                {
                    statusObj.Exception = new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name +
                        "There was a problem getting a view, thus no suppliers could be provided");
                }
                else
                {
                    statusObj.Value = readOnlyCollection;
                }
                return statusObj;
            }
        }
    }
}
