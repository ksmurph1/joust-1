using System;
namespace DAL
{
    public struct DataSpec : IDataSpecs
    {
        public Guid ID {get; internal set;}
        public byte Grade {get; internal set;}
        public ushort Length {get; internal set;}
        public ushort Width {get; internal set;}
        public decimal Price{get; internal set;}

    }
}