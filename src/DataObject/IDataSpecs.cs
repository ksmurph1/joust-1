using System;
namespace DataObject
{
    public interface IDataSpecs
    {
        Guid ID {get;}
        byte Grade {get;}
        ushort Length {get;}
        ushort Width {get;}
        decimal Price{get;}
    }
}