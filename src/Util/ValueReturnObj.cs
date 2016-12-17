namespace Util
{
    public struct ValueReturnObj<T> : IValueReturnObj<T>
    {
       public T Value {get; set;}
       public System.Exception Exception {get; set;}
    }
}