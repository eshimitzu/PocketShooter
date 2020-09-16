namespace Heyworks.PocketShooter.UI.Localization
{
    /// <summary>
    /// Represents an object providing localized representation of date time.
    /// </summary>
    public sealed class DateTimeLocalizationFormater : IDateTimeFormater
    {
        /// <summary>
        /// Gets formatted short-hand representation of days.
        /// </summary>
        /// <param name="daysCount"> Number of days. </param>
        public string GetDaysFormattedShort(int daysCount)
        {
            return string.Format(GetTranslation(LocKeys.Time.DAYS_TEMPLATE_SHORT), daysCount);
        }

        /// <summary>
        /// Gets formatted short-hand representation of hours.
        /// </summary>
        /// <param name="hoursCount"> Number of hours. </param>
        public string GetHoursFormattedShort(int hoursCount)
        {
            return string.Format(GetTranslation(LocKeys.Time.HOURS_TEMPLATE_SHORT), hoursCount);
        }

        /// <summary>
        /// Gets formatted short-hand representation of minutes.
        /// </summary>
        /// <param name="minutesCount"> Number of minutes. </param>
        public string GetMinutesFormattedShort(int minutesCount)
        {
            return string.Format(GetTranslation(LocKeys.Time.MINUTES_TEMPLATE_SHORT), minutesCount);
        }

        /// <summary>
        /// Gets formatted short-hand representation of seconds.
        /// </summary>
        /// <param name="secondsCount"> Number of seconds. </param>
        public string GetSecondsFormattedShort(int secondsCount)
        {
            return string.Format(GetTranslation(LocKeys.Time.SECONDS_TEMPLATE_SHORT), secondsCount);
        }

        /// <summary>
        /// Gets formatted short-hand representation of days together with hours.
        /// </summary>
        /// <param name="daysCount"> Number of days. </param>
        /// <param name="hoursCount"> Number of hours. </param>
        public string GetDaysHoursFormattedShort(int daysCount, int hoursCount)
        {
            return string.Format(GetTranslation(LocKeys.Time.DAYS_HOURS_TEMPLATE_SHORT), daysCount, hoursCount);
        }

        /// <summary>
        /// Gets formatted short-hand representation of hours together with minutes.
        /// </summary>
        /// <param name="hoursCount"> Number of hours. </param>
        /// /// <param name="minutesCount"> Number of minutes. </param>
        public string GetHoursMinutesFormattedShort(int hoursCount, int minutesCount)
        {
            return string.Format(GetTranslation(LocKeys.Time.HOURS_MINUTES_TEMPLATE_SHORT), hoursCount, minutesCount);
        }

        /// <summary>
        /// Gets formatted short-hand representation of minutes together with seconds.
        /// </summary>
        /// /// <param name="minutesCount"> Number of minutes. </param>
        /// /// <param name="secondsCount"> Number of seconds. </param>
        public string GetMinutesSecondsFormattedShort(int minutesCount, int secondsCount)
        {
            return string.Format(GetTranslation(LocKeys.Time.MINUTES_SECONDS_TEMPLATE_SHORT), minutesCount, secondsCount);
        }

        /// <summary>
        /// Gets formatted full-length representation of months.
        /// </summary>
        /// /// <param name="monthsCount"> Number of months. </param>
        public string GetMonthsFormattedLong(int monthsCount)
        {
            return monthsCount > 1 ? string.Format(GetTranslation(LocKeys.Time.MONTHS_TEMPLATE), monthsCount)
                : GetTranslation(LocKeys.Time.ONE_MONTH);
        }

        /// <summary>
        /// Gets formatted full-length representation of weeks.
        /// </summary>
        /// /// <param name="weeksCount"> Number of weeks. </param>
        public string GetWeeksFormattedLong(int weeksCount)
        {
            return weeksCount > 1
                ? string.Format(GetTranslation(LocKeys.Time.WEEKS_TEMPLATE), weeksCount)
                : GetTranslation(LocKeys.Time.ONE_WEEK);
        }

        /// <summary>
        /// Gets formatted full-length representation of days.
        /// </summary>
        /// /// <param name="daysCount"> Number of days. </param>
        public string GetDaysFormattedLong(int daysCount)
        {
            return daysCount > 1
                ? string.Format(GetTranslation(LocKeys.Time.DAYS_TEMPLATE), daysCount)
                : GetTranslation(LocKeys.Time.ONE_DAY);
        }

        /// <summary>
        /// Gets formatted full-length representation of hours.
        /// </summary>
        /// /// <param name="hoursCount"> Number of hours. </param>
        public string GetHoursFormattedLong(int hoursCount)
        {
            return hoursCount > 1
                ? string.Format(GetTranslation(LocKeys.Time.HOURS_TEMPLATE), hoursCount)
                : GetTranslation(LocKeys.Time.ONE_HOUR);
        }

        /// <summary>
        /// Gets formatted full-length representation of minutes.
        /// </summary>
        /// /// <param name="minutesCount"> Number of minutes. </param>
        public string GetMinutesFormattedLong(int minutesCount)
        {
            return minutesCount > 1
                ? string.Format(GetTranslation(LocKeys.Time.MINUTES_TEMPLATE), minutesCount)
                : GetTranslation(LocKeys.Time.ONE_MINUTE);
        }

        /// <summary>
        /// Gets formatted full-length representation of seconds.
        /// </summary>
        /// /// <param name="secondsCount"> Number of seconds. </param>
        public string GetSecondsFormattedLong(int secondsCount)
        {
            return secondsCount > 1
                ? string.Format(GetTranslation(LocKeys.Time.SECONDS_TEMPLATE), secondsCount)
                : GetTranslation(LocKeys.Time.ONE_SECOND);
        }

        private static string GetTranslation(string key)
        {
            return key.Localized();
        }
    }
}
