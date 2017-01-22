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

        public static object[][] Values = new object[][] {};
        //{

        //new[] {0, 1, 2, 3, 4}.Cast<object>().ToArray(),
        //new[] {0.0, 0.1, 0.2, 0.3, 0.4}.Cast<object>().ToArray(),
        //new[] {0.0f, 0.1f, 0.2f, 0.3f, 0.4f}.Cast<object>().ToArray()
        //};


        //    public static object[][] Min_Max()          => Values.Select(data => new[] {data[MinIndex], data[MaxIndex]}).ToArray();
        //    public static object[][] Min_Max_Inverted() => Values.Select(data => new[] {data[MaxIndex], data[MinIndex]}).ToArray();
        //    public static object[][] Min_Max_Inside()   => Values.Select(data => new[] {data[MinIndex], data[MaxIndex], data[InsideIndex]}).ToArray();
        //    public static object[][] Min_Max_Below()    => Values.Select(data => new[] {data[MinIndex], data[MaxIndex], data[BelowMinIndex]}).ToArray();
        //    public static object[][] Min_Max_Above()    => Values.Select(data => new[] {data[MinIndex], data[MaxIndex], data[AboveMaxIndex]}).ToArray();
        //    public static object[][] Min_Max_Min()      => Values.Select(data => new[] {data[MinIndex], data[MaxIndex], data[MinIndex]}).ToArray();
        //    public static object[][] Min_Max_Max()      => Values.Select(data => new[] {data[MinIndex], data[MaxIndex], data[MaxIndex]}).ToArray();
    }
}
