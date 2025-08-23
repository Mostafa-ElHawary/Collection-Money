using System;
using System.Globalization;

namespace Tracker_Money.Helpers
{
    public static class CurrencyFormatter
    {
        /// <summary>
        /// Formats a decimal amount with the specified currency code
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <param name="currencyCode">The currency code (e.g., USD, EUR)</param>
        /// <returns>Formatted currency string</returns>
        public static string FormatCurrency(decimal amount, string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                currencyCode = "USD";

            // Use the currency code directly without adding additional symbols
            return $"{amount:N2} {currencyCode}";
        }

        /// <summary>
        /// Formats a decimal amount with the specified currency code using culture-specific formatting
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <param name="currencyCode">The currency code (e.g., USD, EUR)</param>
        /// <param name="culture">The culture to use for formatting</param>
        /// <returns>Formatted currency string</returns>
        public static string FormatCurrency(decimal amount, string currencyCode, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                currencyCode = "USD";

            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            return $"{amount.ToString("N2", culture)} {currencyCode}";
        }

        /// <summary>
        /// Formats a decimal amount with the specified currency code for display in tables
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <param name="currencyCode">The currency code (e.g., USD, EUR)</param>
        /// <returns>Formatted currency string with appropriate CSS classes</returns>
        public static string FormatCurrencyForDisplay(decimal amount, string currencyCode)
        {
            var formattedAmount = FormatCurrency(amount, currencyCode);
            var cssClass = amount >= 0 ? "text-success" : "text-danger";
            return $"<span class=\"{cssClass} fw-bold\">{formattedAmount}</span>";
        }

        /// <summary>
        /// Parses a currency string back to decimal
        /// </summary>
        /// <param name="currencyString">The currency string to parse</param>
        /// <param name="currencyCode">The expected currency code</param>
        /// <returns>The parsed decimal amount</returns>
        public static decimal ParseCurrency(string currencyString, string currencyCode = "USD")
        {
            if (string.IsNullOrWhiteSpace(currencyString))
                return 0m;

            // Remove currency code and any non-numeric characters except decimal point and minus
            var numericPart = currencyString.Replace(currencyCode, "").Trim();
            numericPart = numericPart.Replace(",", ""); // Remove thousands separators

            if (decimal.TryParse(numericPart, out decimal result))
                return result;

            return 0m;
        }

        /// <summary>
        /// Validates if a string is a valid currency code
        /// </summary>
        /// <param name="currencyCode">The currency code to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidCurrencyCode(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                return false;

            // Check if it's a 3-letter currency code
            return currencyCode.Length == 3 && currencyCode.All(char.IsLetter);
        }
    }
}