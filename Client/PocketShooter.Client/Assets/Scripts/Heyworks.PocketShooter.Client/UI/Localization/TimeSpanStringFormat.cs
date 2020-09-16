using System;

/// <summary>
/// Enumeration of all formats of <see cref="TimeSpan"/> representation as string.
/// </summary>
public enum TimeSpanStringFormat
{
    /// <summary>
    /// Formatted representation of time span, taking short-hand only for the most big component of the specified time span.
    /// </summary>
    ShortSingleTimeComponent        = 1,

    /// <summary>
    /// Formatted representation of time span, taking short-hands for the two most big components of the specified time span, without trimming the small component, even if it equals zero.
    /// </summary>
    ShortCoupleTimeComponent        = 2,

    /// <summary>
    /// Formatted representation of time span, taking short-hands for the two most big components of the specified time span, trimming the small component, if it equals zero.
    /// </summary>
    ShortCoupleTimeComponentTrimmed = 3,

    /// <summary>
    /// Formatted representation of time span, taking full name only for the most big component of the specified time span.
    /// </summary>
    LongSingleTimeComponent         = 4
}
