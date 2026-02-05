using System;
namespace Game.Core
{
    [Flags] // Bu Sayede Yönlerini Birleştireceğiz (Örn : Up | Down)
    public enum MoveDirection
    {
        None = 0,
        Up = 1 << 0, // 1
        Down = 1 << 1, // 2
        Left = 1 << 2, // 2
        Right = 1 << 3, // 3
        All = Up | Down | Left | Right,
        Horizontal = Left | Right,
        Vertical = Up | Down
    }
}