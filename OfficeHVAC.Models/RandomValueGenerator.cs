using System;

namespace OfficeHVAC.Models
{
    public class RandomValueGenerator : IRandomValueGenerator
    {
        private readonly Random _random;

        public RandomValueGenerator()
        {
            _random = new Random();
        }

        public RandomValueGenerator(int seed)
        {
            _random = new Random(seed);
        }


        public double Next(int min, int max) => _random.Next(min, max);

        public double NextDouble() => _random.NextDouble();
    }
}
