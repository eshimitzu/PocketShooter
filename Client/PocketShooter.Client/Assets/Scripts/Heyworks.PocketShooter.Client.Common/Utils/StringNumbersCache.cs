using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Utils
{
    /// <summary>
    /// Represents cash of numbers in string format. Is used for timers in order not to create garbage. 
    /// </summary>
    public class StringNumbersCache
    {
        private readonly Dictionary<int,string> numbers;
        private readonly bool fillWithZeros;
        private readonly int numberOfDigits;

        private StringBuilder stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNumbersCache"/> class.
        /// </summary>
        public StringNumbersCache()
        {
            numbers = new Dictionary<int, string>();
            fillWithZeros = false;
            numberOfDigits = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNumbersCache" /> class.
        /// </summary>
        /// <param name="start">The start number.</param>
        /// <param name="end">The end number.</param>
        /// <param name="fillWithZeros">if set to <c>true</c> fills small number with zeros on the left.</param>
        /// <param name="numberOfDigits">The number of digits.</param>
        public StringNumbersCache(int start, int end, bool fillWithZeros = false, int numberOfDigits = -1)
        {
            Assert.IsTrue(end > start, $"Invalid parameters: {nameof(end)} must be greater then {nameof(start)}");
            
            this.fillWithZeros = fillWithZeros;

            if (fillWithZeros && numberOfDigits == -1)
            {
                this.numberOfDigits = end.ToString().Length;
            }
            else
            {
                this.numberOfDigits = numberOfDigits;
            }

            numbers = new Dictionary<int, string>(end - start + 1);
            GenerateCache(start, end);
        }

        /// <summary>
        /// Gets the string that represents the number.
        /// </summary>
        /// <param name="number">The number.</param>
        public string GetString(int number)
        {
            if (!numbers.ContainsKey(number))
            {
                AddNumber(number);
            }

            return numbers[number];
        }

        private void GenerateCache(int start, int end)
        {
            for (int i = start; i < end + 1; i++)
            {
                AddNumber(i);
            }
        }

        private void AddNumber(int i)
        {
            var str =i.ToString();
            if (fillWithZeros)
            {
                str = FillWithZeros(str);
            }

            numbers[i] = string.Intern(str);
        }

        private string FillWithZeros(string str)
        {
            stringBuilder = stringBuilder ?? new StringBuilder();
            for (var i = str.Length; i < numberOfDigits; i++)
            {
                stringBuilder.Append("0");
            }

            stringBuilder.Append(str);

            var result = stringBuilder.ToString();
            stringBuilder.Clear();

            return result;
        }
    }
}
