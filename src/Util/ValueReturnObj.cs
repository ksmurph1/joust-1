namespace Util
{
    public struct ValueReturnObj<T> : IValueReturnObj<T>
    {
        private T val;
        private System.Exception except;

       public T Value { get { return val; } set { HasVal = true; val = value; } }
       public bool HasVal { get; set; }
       public System.Exception Exception { get { return except; }
                                           set { HasVal = false; except = value; } }
    }
}