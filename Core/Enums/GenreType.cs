using System;

namespace Core.Enums
{
    [Flags]
    public enum GenreType
    {
        None = 0,
        Action = 1,
        Drama = 2,
        Comedy = 4,
        Horror = 8,
        SciFi = 16,
        Documentary = 32,
        Thriller = 64,
        Fantasy = 128,
        Animation = 256,
        Adventure = 512,
        Crime = 1024,
        Family = 2048,
        Mystery = 4096,
        Romance = 8192,
        Western = 16384,
        War = 32768,
        History = 65536,
        Music = 131072
    }
}