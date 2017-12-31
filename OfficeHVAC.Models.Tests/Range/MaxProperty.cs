#pragma warning disable xUnit1015 // MemberData must reference an existing member

using Shouldly;
using System;
using Xunit;

namespace OfficeHVAC.Models.Tests.Range
{
    public abstract partial class RangeTestBase<T> where T : IComparable<T>
    {
        [Theory]
        [MemberData("Min_Max")]
        public void Max_getter_returns_the_value_of_max(T min, T max)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var returnedMax = range.Max;

            //Assert
            returnedMax.ShouldBe(max);
        }

        [Theory]
        [MemberData("Min_Max_Inside")]
        [MemberData("Min_Max_Above")]
        public void Max_setter_stores_new_value(T min, T max, T newMax)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act 
            range.Max = newMax;

            //Assert
            range.Max.ShouldBe(newMax);
        }

        [Theory]
        [MemberData("Min_Max_Below")]
        public void Max_setter_throws_ArgumentOutOfRangeException_when_value_is_smaller_or_equal_to_min(T min, T max, T invalidMax)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act & Assert
            var ex = Should.Throw<ArgumentOutOfRangeException>(() => range.Max = invalidMax);
            ex.ParamName.ShouldBe(nameof(Range<T>.Max));
            ex.ActualValue.ShouldBe(invalidMax);
        }
    }
}
#pragma warning restore xUnit1015 // MemberData must reference an existing member
