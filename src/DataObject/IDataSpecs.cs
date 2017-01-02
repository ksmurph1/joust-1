using System;
namespace DataObject
{
    public interface IDataSpecs
    {
        Guid ID { get; set; }
        byte Grade { get; set; }
        ushort Length { get; set; }
        ushort Width { get; set; }
        decimal Price{ get; set; }
    }
}