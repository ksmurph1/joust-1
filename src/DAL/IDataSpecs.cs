using System;
namespace DAL
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