using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Joust;
using Util;
using Supplier;
using DataObject;
using System.Threading.Tasks;

namespace DotNetCore.Joust
{
    using kvp = KeyValuePair<double, IDataSpecs>;

    public sealed class LowestCostCalculator : IJoust
    {
        //0,sqftreq 1,roomcount 2,hourlylabor 3,desiredgrade
        private enum InputNames : byte { SQFTREQ = 0, ROOMCNT, HRLYLABOR, GRADE };

        public IQuote GetQuote(int[] input)
        {
            // company name to price index map where sqft is less than sqft needed
            ConcurrentStack<KeyValuePair<string, kvp>> minSqFtSuppliers = new ConcurrentStack<KeyValuePair<string, kvp>>();

            // company name to price index pair where sqft is greater equal than sqft needed 
            KeyValuePair<string, kvp> maxSqFtSupplier = new KeyValuePair<string, kvp>(String.Empty, new kvp(Double.MaxValue, new DataSpec()));
            // check if exception on getting views
            IValueReturnObj<object> statusObj;
            AllSupplierView view=new AllSupplierView(out statusObj);
            if (statusObj.Exception != null)
            {
                // if exception then throw
                throw statusObj.Exception;
            }
            ConcurrentStack<Task> taskList = new ConcurrentStack<Task>();
           Parallel.ForEach (view.Suppliers.Value, (supplier)=>
            {
                    
                    Task thisTask=Task.Factory.StartNew(()=>
                    {
                        // get suppliers that are under sqft requirement but meet other criteria
                    supplier.SetCriteria((IDataSpecs d) => d.Grade >= input[(byte)InputNames.GRADE] &&
                    d.Width * d.Length < input[(byte)InputNames.SQFTREQ]);
                    IEnumerable<kvp> transformedResult=SegmentData(supplier);
                    // save min sq ft suppliers for later processing
                    if (transformedResult.Count() > 0)
                        minSqFtSuppliers.PushRange(transformedResult.Select(
                            pi => new KeyValuePair<string, kvp>(supplier.CompanyName, pi)).ToArray());
                    }).ContinueWith(t=>
                    {
                    // get suppliers that are equal or over sqft requirement but meet other criteria
                    // set condition for grade and inventory for sqfootneeded >= sqfoot required
                    supplier.SetCriteria((IDataSpecs d) => d.Grade >= input[(byte)InputNames.GRADE] &&
                    d.Width * d.Length >= input[(byte)InputNames.SQFTREQ]);
                    IEnumerable<kvp> transformedResult=SegmentData(supplier);
                        // order by ascending max of min is the best that meets criteria
                        // order by sqft then by price index
                        kvp thisMax = transformedResult.OrderBy(col => col.Value.Width*col.Value.Length).
                                    ThenBy(col=>col.Key).DefaultIfEmpty(new kvp(Double.MaxValue, new DataSpec())).First();
                    
                        lock(taskList)
                        {
                            // this must be 1 synchronous operation
                        if (maxSqFtSupplier.Value.Key > thisMax.Key)
                        {
                            maxSqFtSupplier = new KeyValuePair<string, kvp>(supplier.CompanyName, thisMax);
                        }
                        }
                    },TaskContinuationOptions.NotOnFaulted);
                    taskList.Push(thisTask);
           });
            Task.WaitAll(taskList.ToArray());  // wait for this stage of processing to complete
            uint totalSqFt = 0;

            // most bang for the buck 1st, next 2nd and so on, so add from there to find best price
            //order by price index ascending then by sqft descending get most bang for buck
            KeyValuePair<string, kvp>[] best = minSqFtSuppliers.OrderBy(cpi => cpi.Value.Key).
              ThenByDescending(cpi=>cpi.Value.Value.Length*cpi.Value.Value.Width).TakeWhile(cpi =>
              {
                  totalSqFt += (uint)cpi.Value.Value.Length * cpi.Value.Value.Width;
                  // take while we haven't met sqft qouta
                  return totalSqFt < input[(byte)InputNames.SQFTREQ];
              }).ToArray();
            if (totalSqFt < input[(byte)InputNames.SQFTREQ])
            {
                // we ran out of suppliers before meeting qouta
                // best is the max found that meets sqft
                best = new KeyValuePair<string, kvp>[] { maxSqFtSupplier };
            }

            // calculate new Qoute based on lowest price
            return new Qoute(input[(byte)InputNames.HRLYLABOR], input[(byte)InputNames.ROOMCNT],
                             best.Select(cpi => cpi.Value.Value.ID).ToArray());
           
        }
        private IEnumerable<kvp> SegmentData(IInventory supplier)
        {
            // get inventory with grade >= input and square footage less than what is required
            IEnumerable<kvp> priceIdxs;
            // get price index
            IValueReturnObj<System.Collections.ObjectModel.ReadOnlyCollection<kvp>> resultObj =
            supplier.ApplyOperation(d => (double)d.Price / (double)(d.Width * d.Length));
            if (resultObj.HasVal)
            {
            // find lowest price index-most bang for the buck
            priceIdxs = resultObj.Value;
            }
            // throw exception otherwise
            else
            {
                throw new Exception("SegmentData(IInventory,Func<IDataSpecs,bool>):"+resultObj.Exception);
            }
          
            return priceIdxs;
        }
    }
}