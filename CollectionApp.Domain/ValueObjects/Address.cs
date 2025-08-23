using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CollectionApp.Domain.Common;

namespace CollectionApp.Domain.ValueObjects
{
    public sealed class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string PostalCode { get; }

        public Address(string street, string city, string state, string country, string postalCode)
        {
            Street = ValidateRequired(street, nameof(street));
            City = ValidateRequired(city, nameof(city));
            State = ValidateRequired(state, nameof(state));
            Country = ValidateRequired(country, nameof(country));
            PostalCode = ValidatePostal(postalCode);
        }

        private static string ValidateRequired(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{paramName} is required.", paramName);
            return value.Trim();
        }

        private static string ValidatePostal(string postal)
        {
            if (string.IsNullOrWhiteSpace(postal)) throw new ArgumentException("PostalCode is required.", nameof(postal));
            var normalized = postal.Trim().ToUpperInvariant();
            if (normalized.Length < 3 || normalized.Length > 10) throw new ArgumentException("PostalCode length invalid.", nameof(postal));
            if (!Regex.IsMatch(normalized, "^[A-Z0-9 -]+$") ) throw new ArgumentException("PostalCode format invalid.", nameof(postal));
            return normalized;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return PostalCode;
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {State}, {Country} {PostalCode}";
        }
    }
}

