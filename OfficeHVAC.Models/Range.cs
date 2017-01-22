using System;

namespace OfficeHVAC.Models
{
    public class Range<T> : IRange<T> where T : IComparable<T>
    {
        private T min;
        private T max;

        public Range(T min, T max)
        {
            if(min.IsGreaterOrEqualTo(max))
                throw new ArgumentException("Min must be less than Max", nameof(Min));
            this.min = min;
            this.max = max;
        }

        public T Min
        {
            get { return min; }
            set
            {
                if (value.IsLessThan(max))
                    min = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(Min), value, $"New minimum value must be less than maximum: {Max}");
            }
        }

        public T Max
        {
            get { return max; }
            set
            {
                if (value.IsGreaterThan(min))
                    max = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(Max), value, $"New maximum value must be greater than minimum: {Min}");
            }
        }

        public bool Contains(T item, bool open = true)
        {
            if (open)
                return item.IsGreaterThan(min) && item.IsLessThan(max);
            return item.IsGreaterOrEqualTo(min) && item.IsLessOrEqualTo(max);
        }
    }

    public static class ComparableExtensions
    {
        internal static bool IsLessThan<T>(this IComparable<T> a, T b) => a.CompareTo(b) < 0;
        internal static bool IsLessOrEqualTo<T>(this IComparable<T> a, T b) => a.CompareTo(b) <= 0;
        internal static bool AreEqual<T>(this IComparable<T> a, T b)      => a.CompareTo(b) == 0;
        internal static bool IsGreaterThan<T>(this IComparable<T> a, T b) => a.CompareTo(b) > 0;
        internal static bool IsGreaterOrEqualTo<T>(this IComparable<T> a, T b) => a.CompareTo(b) >= 0;

    }
}
