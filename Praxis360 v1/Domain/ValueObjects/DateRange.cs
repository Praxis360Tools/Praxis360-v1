using System;

namespace Praxis360_v1.Domain.ValueObjects;

public sealed record DateRange
{
    public DateOnly Start { get; }
    public DateOnly? End { get; }

    public DateRange(DateOnly start, DateOnly? end = null)
    {
        if (start == default) throw new ArgumentException("Start date must be specified.", nameof(start));
        if (end.HasValue && end.Value < start) throw new ArgumentException("End date cannot be earlier than start date.", nameof(end));

        Start = start;
        End = end;
    }

    public bool Contains(DateOnly date)
    {
        if (date < Start) return false;
        if (End.HasValue && date > End.Value) return false;
        return true;
    }

    public override string ToString() => End.HasValue ? $"{Start:yyyy-MM-dd}..{End:yyyy-MM-dd}" : $"{Start:yyyy-MM-dd}..";
}
