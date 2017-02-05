using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private System.Collections.Concurrent.ConcurrentStack<IDataSpecs> rows = 
                                               new System.Collections.Concurrent.ConcurrentStack<IDataSpecs>();
        private System.Threading.ReaderWriterLockSlim writeReadLock=new System.Threading.ReaderWriterLockSlim();
        private string title;
        public string CompanyName { get { return title; } }
        private delegate bool UnaryPred<T>(T arg);
        private Object conditional;
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
                            invHolder.rows.PushRange(rStatusObj.Value);

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
                statusObj.Value= rows.First(d => d.ID.Equals(id));
               
            }
            catch (Exception ex)
            {
                statusObj.Exception = new Exception("GetRow(" + id+ "): " + ex.Message);
            }
            return statusObj;
        }
        public void SetCriteria<T>(Func<T, bool> criteria)
        {
            // allow many concurrent reads but exclusivity on write
            writeReadLock.EnterWriteLock();
            this.conditional = new UnaryPred<T>(criteria);
            writeReadLock.ExitWriteLock();
        }

        public IValueReturnObj<ReadOnlyCollection<KeyValuePair<T, IDataSpecs>>> ApplyOperation<T>(Func<IDataSpecs, T> operation)
        {
            IValueReturnObj<ReadOnlyCollection<KeyValuePair<T, IDataSpecs>>> statusObj = new ValueReturnObj<ReadOnlyCollection<KeyValuePair<T, IDataSpecs>>>();
                try
                {
                    IEnumerable<IDataSpecs> collection=rows.AsEnumerable();
                    writeReadLock.EnterReadLock();// synchronize so conditional variable so writes and reads don't conflict
                    // set conditional for rows to select and return new collection
                    if (conditional != null)
                    {
                        collection = collection.Where(d => ((UnaryPred<IDataSpecs>)conditional)(d));
                    }
                    writeReadLock.ExitReadLock();
                    statusObj.Value=new ReadOnlyCollection<KeyValuePair<T, IDataSpecs>>(
                        collection.AsParallel().Select(
                             // apply operation on each data object and store as key, value
                        d => new KeyValuePair<T, IDataSpecs>(operation(d), d)
                        ).ToArray());
                }
                catch (Exception e)
                {

                    statusObj.Exception = new Exception("ApplyOperation<T>("+operation + "): " + e.Message);
                }
                finally
                {
                    // if exception make sure write reader lock is exited
                    if (writeReadLock.IsReadLockHeld)
                    {
                        writeReadLock.ExitReadLock();
                    }
                }
            return statusObj;
        }

    }
}