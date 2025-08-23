using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using CollectionApp.Domain.Common;

namespace CollectionApp.Domain.ValueObjects
{
    public sealed class Money : ValueObject, IComparable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }
        
        // Property to access the Amount value directly
        public decimal Value => Amount;

        public static readonly string DefaultCurrency = "USD";

        public Money(decimal amount, string? currency = null)
        {
            if (amount < 0m) throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
            currency = string.IsNullOrWhiteSpace(currency) ? DefaultCurrency : currency.Trim().ToUpperInvariant();
            if (currency.Length is < 3 or > 3) throw new ArgumentException("Currency must be a 3-letter ISO code.", nameof(currency));
            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
            Currency = currency;
        }

        public static Money Zero(string? currency = null) => new Money(0m, currency);

        protected override System.Collections.Generic.IEnumerable<object?> GetEqualityComponents()
        {
            yield return Currency;
            yield return Amount;
        }

        public override string ToString()
        {
            return string.Create(CultureInfo.InvariantCulture, $"{Currency} {Amount:N2}");
        }

        public int CompareTo(Money? other)
        {
            if (other is null) return 1;
            EnsureSameCurrency(other);
            return Amount.CompareTo(other.Amount);
        }

        private void EnsureSameCurrency(Money other)
        {
            if (!string.Equals(Currency, other.Currency, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Currency mismatch between Money values.");
            }
        }

        public static Money operator +(Money left, Money right)
        {
            left.EnsureSameCurrency(right);
            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            left.EnsureSameCurrency(right);
            var result = left.Amount - right.Amount;
            if (result < 0m) throw new InvalidOperationException("Money subtraction cannot produce negative amount.");
            return new Money(result, left.Currency);
        }

        public static Money operator *(Money left, decimal multiplier)
        {
            if (multiplier < 0m) throw new ArgumentOutOfRangeException(nameof(multiplier));
            return new Money(decimal.Round(left.Amount * multiplier, 2, MidpointRounding.AwayFromZero), left.Currency);
        }

        public static Money operator /(Money left, decimal divisor)
        {
            if (divisor <= 0m) throw new ArgumentOutOfRangeException(nameof(divisor));
            return new Money(decimal.Round(left.Amount / divisor, 2, MidpointRounding.AwayFromZero), left.Currency);
        }

        public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
        public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
        public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;
        public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;

        public IReadOnlyList<Money> AllocateEvenly(int parts)
        {
            if (parts <= 0) throw new ArgumentOutOfRangeException(nameof(parts));
            var basePart = Math.Floor(Amount / parts * 100m) / 100m;
            var amounts = Enumerable.Repeat(new Money((decimal)basePart, Currency), parts).ToArray();
            var remainder = Amount - basePart * parts; // remainder in decimal
            var cents = (int)Math.Round(remainder * 100m, MidpointRounding.AwayFromZero);
            for (int i = 0; i < cents; i++)
            {
                amounts[i] = amounts[i] + new Money(0.01m, Currency);
            }
            return Array.AsReadOnly(amounts);
        }
    }
}

