using System;

namespace OfficeHVAC.Models
{
    public interface IRandomValueGenerator
    {
        /// <summary>
        /// Returns random integer from [min, max) range.
        /// </summary>
        /// <param name="min">Inclusive, minimal value of returned integer.</param>
        /// <param name="max">Exclusive, maximal value of returned integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if min is larger than max.</exception>
        double Next(int min, int max);

        /// <summary>
        /// Returns a double from range 0.0 to 1.0 excluding 1.0.
        /// </summary>
        double NextDouble();
    }
}