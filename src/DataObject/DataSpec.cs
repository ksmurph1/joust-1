using System;
namespace DataObject
{
    public struct DataSpec : IDataSpecs
    {
        public Guid ID {get; set;}
        public byte Grade {get; set;}
        public ushort Length {get; set;}
        public ushort Width {get; set;}
        public decimal Price{get; set;}

    }
}