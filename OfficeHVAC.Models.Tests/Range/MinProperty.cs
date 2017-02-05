using Shouldly;
using System;
using Xunit;

namespace OfficeHVAC.Models.Tests.Range
{
    public abstract partial class RangeTestBase<T> where T : IComparable<T>
    {
        [Theory]
        [MemberData("Min_Max")]
        public void Min_getter_returns_the_value_of_min(T min, T max)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var returnedMin = range.Min;

            //Assert
            returnedMin.ShouldBe(min);
        }

        [Theory]
        [MemberData("Min_Max_Inside")]
        [MemberData("Min_Max_Below")]
        public void Min_setter_stores_new_value(T min, T max, T newMin)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act 
            range.Min = newMin;

            //Assert
            range.Min.ShouldBe(newMin);
        }

        [Theory]
        [MemberData("Min_Max_Above")]
        public void Min_setter_throws_ArgumentOutOfRangeException_when_value_is_greater_or_equal_to_max(T min, T max, T invalidMin)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act & Assert
            var ex = Should.Throw<ArgumentOutOfRangeException>(() => range.Min = invalidMin);
            ex.ParamName.ShouldBe(nameof(Range<T>.Min));
            ex.ActualValue.ShouldBe(invalidMin);
        }
    }
}
