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
            KeyValuePair<string, kvp> maxSqFtSupplier = new KeyValuePair<string, kvp>(String.Empty, new kvp(Double.MinValue, new DataSpec()));
            foreach (IInventory supplier in new AllSupplierView().Suppliers)
            {
                    Func<IDataSpecs, bool> cond = (d) => d.Grade >= input[(byte)InputNames.GRADE] &&
                    d.Width * d.Length < input[(byte)InputNames.SQFTREQ];
                    Task<IEnumerable<kvp>> under = Task<IEnumerable<kvp>>.Factory.StartNew(() => SegmentData(supplier, cond));
                    under.ContinueWith(t =>
                        minSqFtSuppliers.PushRange(t.Result.Select(pi => new KeyValuePair<string, kvp>(supplier.CompanyName, pi)).ToArray()));


                    // set condition for grade and inventory for sqfootneeded >= sqfoot required
                    cond = (d) => d.Grade >= input[(byte)InputNames.GRADE] &&
                    d.Width * d.Length >= input[(byte)InputNames.SQFTREQ];
                    Task<IEnumerable<kvp>> over = Task<IEnumerable<kvp>>.Factory.StartNew(() => SegmentData(supplier, cond));
                    over.ContinueWith(t =>
                    {
                        kvp thisMax = t.Result.OrderByDescending(col => col.Key).First();
                        // convert to reference for atomic operation
                        Nullable<KeyValuePair<string, kvp>> temp = maxSqFtSupplier;
                        if (temp.Value.Value.Key < thisMax.Key)
                        {
                            maxSqFtSupplier = new KeyValuePair<string, kvp>(supplier.CompanyName, thisMax);
                        }

                    });
            }
            uint totalSqFt = 0;

            // most bang for the buck 1st, next 2nd and so on, so add from there to find best price
            IEnumerable<KeyValuePair<string, kvp>> best = minSqFtSuppliers.OrderBy(cpi => cpi.Value.Key).
              TakeWhile(cpi =>
              {
                  totalSqFt += (uint)cpi.Value.Value.Length * cpi.Value.Value.Width;
                  // take while we haven't met sqft qouta
                  return totalSqFt < input[(byte)InputNames.SQFTREQ];
              });
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
        private IEnumerable<kvp> SegmentData(IInventory supplier, Func<IDataSpecs, bool> divider)
        {
            // get inventory with grade >= input and square footage less than what is required
            supplier.SetCriteria(divider);

            // get price index
            IList<IValueReturnObj<kvp>> suppliers =
            supplier.ApplyOperation(d => (double)d.Price / (double)(d.Width * d.Length));

            IEnumerable<kvp> priceIdxs;
            // see if there was a exception
            Exception ex;
            try
            {
                ex = new AggregateException(suppliers.Where(status => status.Exception != null).Select(
                                                     status => status.Exception));
            }
            catch (Exception)
            {
                ex = null;
            }
            // if not notable exception - continue
            if (ex != null)
            {
                // find lowest price index-most bang for the buck
                priceIdxs = suppliers.Select(status => status.Value);
            }
            else
            {
                // if exception, throw it
                throw ex;
            }
            return priceIdxs;
        }
    }
}