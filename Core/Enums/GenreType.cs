using System;

namespace Core.Enums
{
    // Той самий бітовий енам, про який казав тімлід
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
        Animation = 256
        // Можна додавати далі: 512, 1024...
    }
}