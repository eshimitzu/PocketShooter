using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Heyworks.PocketShooter.Utils
{
    /// <summary>
    /// Contains all necessary methods to covert <see cref="int"/> or <see cref="double"/> to the string as price text.
    /// </summary>
    public static class FormatNumberWithDelimiters
    {
        /// <summary>
        /// Get string, representing formatted real number with delimiter dividing each digits' set of specified length.
        /// </summary>
        /// <param name="value"> Value to be formatted. </param>
        /// <param name="orderDelimiter"> Character to be set as delimiter of each digits' group.</param>
        /// <param name="decimalDelimiter"> Character to be set as delimiter for the decimal part of specified number. </param>
        /// <param name="categoryStep"> Number of digits to be divided from each other by specified delimiter. </param>
        /// <param name="decimalDigitsCount"> Number of decimal digits to be included into output string. </param>
        public static string GetFormatedDouble(double value, char orderDelimiter = ',', char decimalDelimiter = '.', int categoryStep = 3, int decimalDigitsCount = 2)
        {
            value = Math.Round(value, decimalDigitsCount);

            // Format integer part first.

            var valueIntegerPart = (long)value;
            var integerPartFormatedString = GetFormatedLongInteger(valueIntegerPart, orderDelimiter);

            // Concatenate double part of the number.

            var formatedDouble = integerPartFormatedString;
            var valueText = value.ToString(CultureInfo.InvariantCulture);
            var indexOfDecimalDot = valueIntegerPart.ToString(CultureInfo.InvariantCulture).Length;
            if (indexOfDecimalDot < valueText.Length)
            {
                var decimalDigitsInValue = valueText.Length - (indexOfDecimalDot + 1);
                var decimalPart = decimalDigitsInValue > decimalDigitsCount
                    ? valueText.Substring(indexOfDecimalDot + 1, decimalDigitsCount)
                    : valueText.Substring(indexOfDecimalDot + 1);

                formatedDouble += decimalDelimiter + decimalPart;
            }

            return formatedDouble;
        }

        /// <summary>
        /// Get string, representing formatted long integer number with delimiter dividing each digits' set of specified length.
        /// </summary>
        /// <param name="value"> Value to be formatted. </param>
        /// <param name="delimiter"> Character to be set as delimiter of each digits' group.</param>
        /// <param name="categoryStep"> Number of digits to be divided from each other by specified delimiter. </param>
        public static string GetFormatedLongInteger(long value, char delimiter = ',', int categoryStep = 3)
        {
            var substrings = new List<string>(4);
            var valueText = value.ToString(CultureInfo.InvariantCulture);
            var previousSubstringStartIndex = valueText.Length;
            var currentSubstringStartIndex = previousSubstringStartIndex - categoryStep;
            do
            {
                currentSubstringStartIndex = currentSubstringStartIndex < 0 ? 0 : currentSubstringStartIndex;
                var substringLenght = previousSubstringStartIndex - currentSubstringStartIndex;
                substrings.Add(valueText.Substring(currentSubstringStartIndex, substringLenght));
                previousSubstringStartIndex = currentSubstringStartIndex;
                currentSubstringStartIndex -= categoryStep;
            }
            while (previousSubstringStartIndex > 0);

            var valueStringLength = valueText.Length + (valueText.Length / categoryStep);
            var formatedLongString = new StringBuilder(valueStringLength);
            for (var i = substrings.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    formatedLongString.Append(substrings[i]);
                }
                else
                {
                    formatedLongString.Append(substrings[i] + delimiter);
                }
            }

            return formatedLongString.ToString();
        }

        /// <summary>
        /// Gets percentage representation for a decimal fractional number.
        /// </summary>
        /// <param name="value"> Decimal fractional number. </param>
        public static string GetValuePercentageString(double value)
        {
            return Math.Round(value * 100, 0).ToString(CultureInfo.InvariantCulture);
        }
    }
}
