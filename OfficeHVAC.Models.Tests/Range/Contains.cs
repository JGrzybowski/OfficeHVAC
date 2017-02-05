using Shouldly;
using System;
using Xunit;

namespace OfficeHVAC.Models.Tests.Range
{
    public abstract partial class RangeTestBase<T> where T : IComparable<T>
    {
        [Theory]
        [MemberData("Min_Max_Inside")]
        public void Contains_returns_true_if_item_is_between_min_and_max_in_open_range(T min, T max, T item)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var isContained   = range.Contains(item, open: true);

            //Assert
            isContained.ShouldBeTrue();
        }

        [Theory]
        [MemberData("Min_Max_Inside")]
        public void Contains_returns_true_if_item_is_between_min_and_max_in_closed_range(T min, T max, T item)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var isContained = range.Contains(item, open: false);

            //Assert
            isContained.ShouldBeTrue();
        }

        [Theory]
        [MemberData("Min_Max_Above")]
        public void Contains_returns_false_if_item_is_greater_than_max_in_open_range(T min, T max, T item)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var isContained = range.Contains(item, open: true);

            //Assert
            isContained.ShouldBeFalse();
        }

        [Theory]
        [MemberData("Min_Max_Above")]
        public void Contains_returns_false_if_item_is_greater_than_max_in_closed_range(T min, T max, T item)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var isContained = range.Contains(item, open: false);

            //Assert
            isContained.ShouldBeFalse();
        }

        [Theory]
        [MemberData("Min_Max_Below")]
        public void Contains_returns_false_if_item_is_less_than_min_in_open_range(T min, T max, T item)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var isContained = range.Contains(item, open: true);

            //Assert
            isContained.ShouldBeFalse();
        }

        [Theory]
        [MemberData("Min_Max_Below")]
        public void Contains_returns_false_if_item_is_less_than_min_in_closed_range(T min, T max, T item)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var isContained = range.Contains(item, open: false);

            //Assert
            isContained.ShouldBeFalse();
        }

        [Theory]
        [MemberData("Min_Max_Min")]
        [MemberData("Min_Max_Min")]
        public void Contains_returns_false_if_item_is_equal_to_limit_in_open_range(T min, T max, T item)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var isContained = range.Contains(item, open: true);

            //Assert
            isContained.ShouldBeFalse();
        }

        [Theory]
        [MemberData("Min_Max_Min")]
        [MemberData("Min_Max_Min")]
        public void Contains_returns_true_if_item_is_equal_to_limit_in_closed_range(T min, T max, T item)
        {
            //Arrange
            var range = new Range<T>(min, max);

            //Act
            var isContained = range.Contains(item, open: false);

            //Assert
            isContained.ShouldBeTrue();
        }
    }
}
