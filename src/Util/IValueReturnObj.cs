namespace Util
{
    public interface IValueReturnObj<T>
    {
       T Value {get; set;}
       System.Exception Exception {get; set;}
    }
}