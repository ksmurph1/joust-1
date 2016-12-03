using System;
namespace DAO
{
    public interface IDataSpecs
    {
        GUID ID {get;}
        byte Grade {get;}
        ushort Length {get;}
        ushort Width {get;}
        decimal Price{get;}
    }
}