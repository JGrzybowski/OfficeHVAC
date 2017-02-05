using System;

namespace OfficeHVAC.Models.Tests.Range
{
    public abstract partial class RangeTestBase<T> where T : IComparable<T>
    {
        protected const int BelowMinIndex = 0;
        protected const int MinIndex = 1;
        protected const int InsideIndex = 2;
        protected const int MaxIndex = 3;
        protected const int AboveMaxIndex = 4;   
    }
}
