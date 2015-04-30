using System;

namespace AtNet.Cms.Domain.Interface.Enum
{
    [Flags]
    public enum Direction
    {
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
    }
}
