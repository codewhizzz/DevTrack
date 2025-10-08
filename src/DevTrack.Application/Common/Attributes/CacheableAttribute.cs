using System;

namespace DevTrack.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CacheableAttribute : Attribute
{
    public int DurationInMinutes { get; }

    public CacheableAttribute(int durationInMinutes = 5)
    {
        DurationInMinutes = durationInMinutes;
    }
}