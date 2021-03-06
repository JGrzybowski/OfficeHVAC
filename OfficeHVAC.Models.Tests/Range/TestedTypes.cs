﻿using System.Linq;
using NodaTime;

namespace OfficeHVAC.Models.Tests.Range
{
    public class OfIntegers : RangeTestBase<int>
    {
        public static object[][] Values = new[]
        {
            new[] { 1,  2,  3,  4,  5}.Cast<object>().ToArray(),
            new[] { 0,  1,  2,  3,  4}.Cast<object>().ToArray(),
            new[] {-3, -2, -1,  0,  1}.Cast<object>().ToArray(),
            new[] {-4, -3, -2, -1,  0}.Cast<object>().ToArray(),
            new[] {-5, -4, -3, -2, -1}.Cast<object>().ToArray()
        };

        public static object[][] Min_Max()          => Values.Select(data => new[] { data[MinIndex], data[MaxIndex] }).ToArray();
        public static object[][] Min_Max_Inverted() => Values.Select(data => new[] { data[MaxIndex], data[MinIndex] }).ToArray();
        public static object[][] Min_Max_Inside()   => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[InsideIndex] }).ToArray();
        public static object[][] Min_Max_Below()    => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[BelowMinIndex] }).ToArray();
        public static object[][] Min_Max_Above()    => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[AboveMaxIndex] }).ToArray();
        public static object[][] Min_Max_Min()      => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[MinIndex] }).ToArray();
        public static object[][] Min_Max_Max()      => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[MaxIndex] }).ToArray();
    }

    public class OfDoubles : RangeTestBase<double>
    {
        public static object[][] Values = new[]
        {
            new[] { 0.5,  1.5,  2.5,  3.5,  4.5}.Cast<object>().ToArray(),
            new[] { 0.0,  1.5,  2.5,  3.5,  4.5}.Cast<object>().ToArray(),
            new[] {-3.5, -2.5, -1.5,  0.0,  1.5}.Cast<object>().ToArray(),
            new[] {-4.5, -3.5, -2.5, -1.5,  0.0}.Cast<object>().ToArray(),
            new[] {-5.5, -4.5, -3.5, -2.5, -1.5}.Cast<object>().ToArray()
        };

        public static object[][] Min_Max()          => Values.Select(data => new[] { data[MinIndex], data[MaxIndex] }).ToArray();
        public static object[][] Min_Max_Inverted() => Values.Select(data => new[] { data[MaxIndex], data[MinIndex] }).ToArray();
        public static object[][] Min_Max_Inside()   => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[InsideIndex] }).ToArray();
        public static object[][] Min_Max_Below()    => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[BelowMinIndex] }).ToArray();
        public static object[][] Min_Max_Above()    => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[AboveMaxIndex] }).ToArray();
        public static object[][] Min_Max_Min()      => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[MinIndex] }).ToArray();
        public static object[][] Min_Max_Max()      => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[MaxIndex] }).ToArray();
    }
    
    public class OfStrings : RangeTestBase<string>
    {
        public static object[][] Values = new[]
        {
            new[] {"a","b","c","d","e"}.Cast<object>().ToArray(),
            new[] {"a","b","c","D","E"}.Cast<object>().ToArray(),
            new[] {"aaa","bbb","ccc","ddd","eee"}.Cast<object>().ToArray(),
        };

        public static object[][] Min_Max()          => Values.Select(data => new[] { data[MinIndex], data[MaxIndex] }).ToArray();
        public static object[][] Min_Max_Inverted() => Values.Select(data => new[] { data[MaxIndex], data[MinIndex] }).ToArray();
        public static object[][] Min_Max_Inside()   => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[InsideIndex] }).ToArray();
        public static object[][] Min_Max_Below()    => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[BelowMinIndex] }).ToArray();
        public static object[][] Min_Max_Above()    => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[AboveMaxIndex] }).ToArray();
        public static object[][] Min_Max_Min()      => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[MinIndex] }).ToArray();
        public static object[][] Min_Max_Max()      => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[MaxIndex] }).ToArray();
    }

    public class OfInstants : RangeTestBase<Instant>
    {
        public static object[][] Values = new[]
        {
            new[]
            {
                Instant.FromUtc(2005,02,21,12,00),
                Instant.FromUtc(2005,02,21,15,00),
                Instant.FromUtc(2005,02,24,15,00),
                Instant.FromUtc(2005,03,24,15,00),
                Instant.FromUtc(2008,03,24,15,00)
            }.Cast<object>().ToArray(),
        };

        public static object[][] Min_Max()          => Values.Select(data => new[] { data[MinIndex], data[MaxIndex] }).ToArray();
        public static object[][] Min_Max_Inverted() => Values.Select(data => new[] { data[MaxIndex], data[MinIndex] }).ToArray();
        public static object[][] Min_Max_Inside()   => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[InsideIndex] }).ToArray();
        public static object[][] Min_Max_Below()    => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[BelowMinIndex] }).ToArray();
        public static object[][] Min_Max_Above()    => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[AboveMaxIndex] }).ToArray();
        public static object[][] Min_Max_Min()      => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[MinIndex] }).ToArray();
        public static object[][] Min_Max_Max()      => Values.Select(data => new[] { data[MinIndex], data[MaxIndex], data[MaxIndex] }).ToArray();
    }
}
