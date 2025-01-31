﻿using CleanArchitecture.Core.Abstractions.Guards;
using CSharpFunctionalExtensions;

namespace CleanArchitecture.Core.Weather.ValueObjects
{
    public sealed class Temperature : ValueObject
    {
        private const int AbsoluteZero = -273;

        private Temperature(int celcius)
        {
            Celcius = celcius;
        }

        public int Celcius { get; }
        public int Farenheit => ConvertToFarenheit(Celcius);

        public static Temperature FromCelcius(int celcius)
        {
            Guard.Against.LessThan(celcius, AbsoluteZero, message: "Temperature cannot be below Absolute Zero");
            return new Temperature(celcius);
        }

        public static Temperature FromFarenheit(int farenheit)
        {
            return FromCelcius(ConvertToCelcius(farenheit));
        }

        public static int ConvertToCelcius(int farenheit)
        {
            return (int)Math.Round((farenheit - 32) * (5.0 / 9.0), 0);
        }

        public static int ConvertToFarenheit(int celcius)
        {
            return 32 + (int)Math.Round(celcius / 0.5556, 0);
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Celcius;
        }
    }
}