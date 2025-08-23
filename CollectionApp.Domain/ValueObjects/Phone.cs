using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CollectionApp.Domain.Common;

namespace CollectionApp.Domain.ValueObjects
{
    public sealed class Phone : ValueObject
    {
        public string CountryCode { get; }
        public string AreaCode { get; }
        public string Number { get; }
        public string? PhoneType { get; }

        private static readonly Regex DigitsOnly = new Regex("^\\d+$", RegexOptions.Compiled);

        // Original constructor for backward compatibility
        public Phone(string countryCode, string areaCode, string number)
        {
            CountryCode = ValidateDigits(countryCode, nameof(countryCode), 1, 3);
            AreaCode = ValidateDigits(areaCode, nameof(areaCode), 1, 5);
            Number = ValidateDigits(number, nameof(number), 4, 12);
        }

        // New overloaded constructor for expanded functionality
        public Phone(string phoneNumber, string? phoneType = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("Phone number is required", nameof(phoneNumber));
            
            // Parse the phone number to extract components
            var parsed = ParsePhoneNumber(phoneNumber);
            CountryCode = parsed.CountryCode;
            AreaCode = parsed.AreaCode;
            Number = parsed.Number;
            PhoneType = phoneType?.Trim();
        }

        private static (string CountryCode, string AreaCode, string Number) ParsePhoneNumber(string phoneNumber)
        {
            var normalized = phoneNumber.Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
            
            // Remove leading + if present
            if (normalized.StartsWith("+")) normalized = normalized[1..];

            if (!DigitsOnly.IsMatch(normalized)) throw new ArgumentException("Phone must contain only digits", nameof(phoneNumber));
            if (normalized.Length < 7) throw new ArgumentException("Phone too short", nameof(phoneNumber));

            // Try to intelligently parse the phone number
            // For US numbers: assume country code 1 if not specified
            if (normalized.Length == 10)
            {
                // US number without country code
                return ("1", normalized.Substring(0, 3), normalized.Substring(3));
            }
            else if (normalized.Length == 11 && normalized.StartsWith("1"))
            {
                // US number with country code
                return ("1", normalized.Substring(1, 3), normalized.Substring(4));
            }
            else if (normalized.Length >= 12)
            {
                // International number
                var countryLen = Math.Min(3, Math.Max(1, normalized.Length - 9));
                var cc = normalized.Substring(0, countryLen);
                var remaining = normalized.Substring(countryLen);
                var areaLen = Math.Min(5, Math.Max(1, remaining.Length - 4));
                var ac = remaining.Substring(0, areaLen);
                var num = remaining.Substring(areaLen);
                return (cc, ac, num);
            }
            else
            {
                // Fallback: assume it's a local number
                var areaLen = Math.Min(3, Math.Max(1, normalized.Length - 4));
                return ("1", normalized.Substring(0, areaLen), normalized.Substring(areaLen));
            }
        }

        public static Phone FromInternational(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Phone input required", nameof(input));
            var normalized = input.Replace(" ", string.Empty).Replace("-", string.Empty);
            if (normalized.StartsWith("+")) normalized = normalized[1..];

            if (!DigitsOnly.IsMatch(normalized)) throw new ArgumentException("Phone must contain only digits", nameof(input));
            if (normalized.Length < 7) throw new ArgumentException("Phone too short", nameof(input));

            // naive split: country (1-3), area (up to 5), rest number
            var countryLen = Math.Min(3, Math.Max(1, normalized.Length - 9));
            var cc = normalized.Substring(0, countryLen);
            var remaining = normalized.Substring(countryLen);
            var areaLen = Math.Min(5, Math.Max(1, remaining.Length - 4));
            var ac = remaining.Substring(0, areaLen);
            var num = remaining.Substring(areaLen);
            return new Phone(cc, ac, num);
        }

        private static string ValidateDigits(string value, string name, int min, int max)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} is required.", name);
            var v = value.Trim().TrimStart('+');
            if (!DigitsOnly.IsMatch(v)) throw new ArgumentException($"{name} must be digits.", name);
            if (v.Length < min || v.Length > max) throw new ArgumentOutOfRangeException(name, $"{name} length must be {min}-{max}.");
            return v;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return CountryCode;
            yield return AreaCode;
            yield return Number;
            yield return PhoneType;
        }

        public override string ToString()
        {
            var phoneString = $"+{CountryCode} {AreaCode} {Number}";
            return string.IsNullOrWhiteSpace(PhoneType) ? phoneString : $"{phoneString} ({PhoneType})";
        }
    }
}

