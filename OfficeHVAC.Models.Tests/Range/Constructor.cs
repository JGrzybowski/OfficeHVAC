using Shouldly;
using System;
using Xunit;
namespace OfficeHVAC.Models.Tests.Range
{
    public abstract partial class RangeTestBase<T> where T : IComparable<T>
    {
        [Theory]
        [MemberData("Min_Max")]
        public void ctor_stores_min_and_max_in_properties(T min, T max)
        {
            //Act
            var range = new Range<T>(min, max);

            //Assert
            range.Min.ShouldBe(min);
            range.Max.ShouldBe(max);
        }

        [Theory]
        [MemberData("Min_Max_Inverted")]
        public void ctor_throws_ArgumentException_when_min_is_not_less_than_max(T min, T max)
        {
            var ex = Should.Throw<ArgumentException>(() => new Range<T>(min, max));
            ex.ParamName.ShouldBeOneOf("min", "max", "Min", "Max");
        }
    }
}
