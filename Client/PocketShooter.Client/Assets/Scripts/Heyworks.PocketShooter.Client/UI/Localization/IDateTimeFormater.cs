namespace Heyworks.PocketShooter.UI.Localization
{
    /// <summary>
    /// Represents an object providing formatted representation of date time.
    /// </summary>
    public interface IDateTimeFormater
    {
        /// <summary>
        /// Gets formatted short-hand representation of days.
        /// </summary>
        /// <param name="daysCount"> Number of days. </param>
        string GetDaysFormattedShort(int daysCount);

        /// <summary>
        /// Gets formatted short-hand representation of hours.
        /// </summary>
        /// <param name="hoursCount"> Number of hours. </param>
        string GetHoursFormattedShort(int hoursCount);

        /// <summary>
        /// Gets formatted short-hand representation of minutes.
        /// </summary>
        /// <param name="minutesCount"> Number of minutes. </param>
        string GetMinutesFormattedShort(int minutesCount);

        /// <summary>
        /// Gets formatted short-hand representation of seconds.
        /// </summary>
        /// <param name="secondsCount"> Number of seconds. </param>
        string GetSecondsFormattedShort(int secondsCount);

        /// <summary>
        /// Gets formatted short-hand representation of days together with hours.
        /// </summary>
        /// <param name="daysCount"> Number of days. </param>
        /// <param name="hoursCount"> Number of hours. </param>
        string GetDaysHoursFormattedShort(int daysCount, int hoursCount);

        /// <summary>
        /// Gets formatted short-hand representation of hours together with minutes.
        /// </summary>
        /// <param name="hoursCount"> Number of hours. </param>
        /// /// <param name="minutesCount"> Number of minutes. </param>
        string GetHoursMinutesFormattedShort(int hoursCount, int minutesCount);

        /// <summary>
        /// Gets formatted short-hand representation of minutes together with seconds.
        /// </summary>
        /// /// <param name="minutesCount"> Number of minutes. </param>
        /// /// <param name="secondsCount"> Number of seconds. </param>
        string GetMinutesSecondsFormattedShort(int minutesCount, int secondsCount);

        /// <summary>
        /// Gets formatted full-length representation of months.
        /// </summary>
        /// /// <param name="monthsCount"> Number of months. </param>
        string GetMonthsFormattedLong(int monthsCount);

        /// <summary>
        /// Gets formatted full-length representation of weeks.
        /// </summary>
        /// /// <param name="weeksCount"> Number of weeks. </param>
        string GetWeeksFormattedLong(int weeksCount);

        /// <summary>
        /// Gets formatted full-length representation of days.
        /// </summary>
        /// /// <param name="daysCount"> Number of days. </param>
        string GetDaysFormattedLong(int daysCount);

        /// <summary>
        /// Gets formatted full-length representation of hours.
        /// </summary>
        /// /// <param name="hoursCount"> Number of hours. </param>
        string GetHoursFormattedLong(int hoursCount);

        /// <summary>
        /// Gets formatted full-length representation of minutes.
        /// </summary>
        /// /// <param name="minutesCount"> Number of minutes. </param>
        string GetMinutesFormattedLong(int minutesCount);

        /// <summary>
        /// Gets formatted full-length representation of seconds.
        /// </summary>
        /// /// <param name="secondsCount"> Number of seconds. </param>
        string GetSecondsFormattedLong(int secondsCount);
    }
}
