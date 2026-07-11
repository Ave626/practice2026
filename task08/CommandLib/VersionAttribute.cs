using System;

namespace CommandLib;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}
