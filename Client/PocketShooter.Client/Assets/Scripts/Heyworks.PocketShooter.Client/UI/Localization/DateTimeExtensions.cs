using System;

namespace Heyworks.PocketShooter.UI.Localization
{
    /// <summary>
    /// Represents class-container with helping methods for <see cref="DateTime"/> and <see cref="TimeSpan"/> classes.
    /// </summary>
    public static class DateTimeExtensions
    {
        private static IDateTimeFormater formater = new DateTimeLocalizationFormater();

        /// <summary>
        /// Gets formatted representation of time span, taking short-hand only for the most big component of the specified time span.
        /// </summary>
        /// <param name="span"> Time span to format. </param>
        public static string ToShortSingleTimeComponentString(this TimeSpan span)
        {
            if (span.TotalDays >= 1)
            {
                return formater.GetDaysFormattedShort(span.Days);
            }

            if (span.TotalHours >= 1)
            {
                return formater.GetHoursFormattedShort(span.Hours);
            }

            if (span.TotalMinutes >= 1)
            {
                formater.GetMinutesFormattedShort(span.Minutes);
            }

            return formater.GetSecondsFormattedShort(span.Seconds);
        }

        /// <summary>
        /// Gets formatted representation of time span, taking short-hands for the two most big components of the specified time span.
        /// </summary>
        /// <param name="span"> Time span to format. </param>
        /// <param name="isTrimmed"> Value indicating whether the small component of time should be trimmed away, if equals zero. </param>
        public static string ToShortCoupleTimeComponentString(this TimeSpan span, bool isTrimmed = true)
        {
            if (span.TotalDays >= 1)
            {
                if (isTrimmed && span.Hours == 0)
                {
                    return formater.GetDaysFormattedShort(span.Days);
                }

                return formater.GetDaysHoursFormattedShort(span.Days, span.Hours);
            }

            if (span.TotalHours >= 1)
            {
                if (isTrimmed && span.Minutes == 0)
                {
                    return formater.GetHoursFormattedShort(span.Hours);
                }

                return formater.GetHoursMinutesFormattedShort(span.Hours, span.Minutes);
            }

            if (span.TotalMinutes >= 1)
            {
                if (isTrimmed && span.Seconds == 0)
                {
                    return formater.GetMinutesFormattedShort(span.Minutes);
                }

                return formater.GetMinutesSecondsFormattedShort(span.Minutes, span.Seconds);
            }

            return formater.GetSecondsFormattedShort(span.Seconds);
        }

        /// <summary>
        /// Gets formatted representation of time span, taking full name only for the most big component of the specified time span.
        /// </summary>
        /// <param name="span"> Time span to format. </param>
        public static string ToLongSingleTimeComponentString(this TimeSpan span)
        {
            const int DAYS_IN_MONTH = 30;
            const int DAYS_IN_WEEK = 7;
            var daysPassed = span.TotalDays;

            if (daysPassed >= DAYS_IN_MONTH)
            {
                var monthsCount = (int)(daysPassed / DAYS_IN_MONTH);

                return formater.GetMonthsFormattedLong(monthsCount);
            }

            if (daysPassed >= DAYS_IN_WEEK)
            {
                return formater.GetWeeksFormattedLong((int)(daysPassed / DAYS_IN_WEEK));
            }

            if (daysPassed >= 1)
            {
                return formater.GetDaysFormattedLong((int)daysPassed);
            }

            var hoursPassed = span.Hours;
            if (hoursPassed > 0)
            {
                return formater.GetHoursFormattedLong(hoursPassed);
            }

            var minutesPassed = span.Minutes;
            if (minutesPassed > 0)
            {
                return formater.GetMinutesFormattedLong(minutesPassed);
            }

            return formater.GetSecondsFormattedLong((int)span.TotalSeconds);
        }

        /// <summary>
        /// Gets formatted representation of time span.
        /// </summary>
        /// <param name="span"> Time span to format. </param>
        /// <param name="format"> Value defining the format. </param>
        public static string ToFormattedString(this TimeSpan span, TimeSpanStringFormat format)
        {
            switch (format)
            {
                case TimeSpanStringFormat.ShortSingleTimeComponent:
                    return span.ToShortSingleTimeComponentString();

                case TimeSpanStringFormat.ShortCoupleTimeComponent:
                    return span.ToShortCoupleTimeComponentString(false);

                case TimeSpanStringFormat.ShortCoupleTimeComponentTrimmed:
                    return span.ToShortCoupleTimeComponentString();

                case TimeSpanStringFormat.LongSingleTimeComponent:
                    return span.ToLongSingleTimeComponentString();

                default:
                    return span.ToString();
            }
        }
    }
}
