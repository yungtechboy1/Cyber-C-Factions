﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

/// <summary>
/// Parse a date/time string.
/// 
/// Can handle relative English-written date times like:
///  - "-1 day": Yesterday
///  - "+12 weeks": Today twelve weeks later
///  - "1 seconds": One second later from now.
///  - "5 days 1 hour ago"
///  - "1 year 2 months 3 weeks 4 days 5 hours 6 minutes 7 seconds"
///  - "today": This day at midnight.
///  - "now": Right now (date and time).
///  - "next week"
///  - "last month"
///  - "2010-12-31"
///  - "01/01/2010 1:59 PM"
///  - "23:59:58": Today at the given time.
/// 
/// If the relative time includes hours, minutes or seconds, it's relative to now,
/// else it's relative to today.
/// </summary>
static class RDP
{
    private static string ValidUnits = "year|month|week|day|hour|minute|second";

    /// <summary>
    /// Ex: "last year"
    /// </summary>
    private static readonly Regex _basicRelativeRegex = new Regex(@"^(last|next) +(" + ValidUnits + ")$");

    /// <summary>
    /// Ex: "+1 week"
    /// Ex: " 1week"
    /// </summary>
    private static readonly Regex _simpleRelativeRegex = new Regex(@"^([+-]?\d+) *(" + ValidUnits + ")s?$");

    /// <summary>
    /// Ex: "2 minutes"
    /// Ex: "3 months 5 days 1 hour ago"
    /// </summary>
    private static readonly Regex _completeRelativeRegex = new Regex(@"^(?: *(\d) *(" + ValidUnits + ")s?)+( +ago)?$");

    public static DateTime Parse(string input)
    {
        // Remove the case and trim spaces.
        input = input.Trim().ToLower();

        // Try common simple words like "yesterday".
        var result = TryParseCommonDateTime(input);
        if (result.HasValue)
            return result.Value;

        // Try common simple words like "last week".
        result = TryParseLastOrNextCommonDateTime(input);
        if (result.HasValue)
            return result.Value;

        // Try simple format like "+1 week".
        result = TryParseSimpleRelativeDateTime(input);
        if (result.HasValue)
            return result.Value;

        // Try first the full format like "1 day 2 hours 10 minutes ago".
        result = TryParseCompleteRelativeDateTime(input);
        if (result.HasValue)
            return result.Value;

        // Try parse fixed dates like "01/01/2000".
        return DateTime.Parse(input);
    }

    private static DateTime? TryParseCommonDateTime(string input)
    {
        switch (input)
        {
            case "now":
                return DateTime.Now;
            case "today":
                return DateTime.Today;
            case "tomorrow":
                return DateTime.Today.AddDays(1);
            case "yesterday":
                return DateTime.Today.AddDays(-1);
            default:
                return null;
        }
    }

    private static DateTime? TryParseLastOrNextCommonDateTime(string input)
    {
        var match = _basicRelativeRegex.Match(input);
        if (!match.Success)
            return null;

        var unit = match.Groups[2].Value;
        var sign = string.Compare(match.Groups[1].Value, "next", true) == 0 ? 1 : -1;
        return AddOffset(unit, sign);
    }

    private static DateTime? TryParseSimpleRelativeDateTime(string input)
    {
        var match = _simpleRelativeRegex.Match(input);
        if (!match.Success)
            return null;

        var delta = Convert.ToInt32(match.Groups[1].Value);
        var unit = match.Groups[2].Value;
        return AddOffset(unit, delta);
    }

    private static DateTime? TryParseCompleteRelativeDateTime(string input)
    {
        var match = _completeRelativeRegex.Match(input);
        if (!match.Success)
            return null;

        var values = match.Groups[1].Captures;
        var units = match.Groups[2].Captures;
        var sign = match.Groups[3].Success ? -1 : 1;
        Debug.Assert(values.Count == units.Count);

        var dateTime = UnitIncludeTime(units) ? DateTime.Now : DateTime.Today;

        for (int i = 0; i < values.Count; ++i)
        {
            var value = sign*Convert.ToInt32(values[i].Value);
            var unit = units[i].Value;

            dateTime = AddOffset(unit, value, dateTime);
        }

        return dateTime;
    }

    /// <summary>
    /// Add/Remove years/days/hours... to a datetime.
    /// </summary>
    /// <param name="unit">Must be one of ValidUnits</param>
    /// <param name="value">Value in given unit to add to the datetime</param>
    /// <param name="dateTime">Relative datetime</param>
    /// <returns>Relative datetime</returns>
    private static DateTime AddOffset(string unit, int value, DateTime dateTime)
    {
        switch (unit)
        {
            case "year":
                return dateTime.AddYears(value);
            case "month":
                return dateTime.AddMonths(value);
            case "week":
                return dateTime.AddDays(value * 7);
            case "day":
                return dateTime.AddDays(value);
            case "hour":
                return dateTime.AddHours(value);
            case "minute":
                return dateTime.AddMinutes(value);
            case "second":
                return dateTime.AddSeconds(value);
            default:
                throw new Exception("Internal error: Unhandled relative date/time case.");
        }
    }

    /// <summary>
    /// Add/Remove years/days/hours... relative to today or now.
    /// </summary>
    /// <param name="unit">Must be one of ValidUnits</param>
    /// <param name="value">Value in given unit to add to the datetime</param>
    /// <returns>Relative datetime</returns>
    private static DateTime AddOffset(string unit, int value)
    {
        var now = UnitIncludesTime(unit) ? DateTime.Now : DateTime.Today;
        return AddOffset(unit, value, now);
    }

    private static bool UnitIncludeTime(CaptureCollection units)
    {
        foreach (Capture unit in units)
            if (UnitIncludesTime(unit.Value))
                return true;
        return false;
    }

    private static bool UnitIncludesTime(string unit)
    {
        switch (unit)
        {
            case "hour":
            case "minute":
            case "second":
                return true;

            default:
                return false;
        }
    }
}