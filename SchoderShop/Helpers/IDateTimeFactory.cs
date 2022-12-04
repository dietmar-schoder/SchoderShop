using System;

namespace SchoderShop.Helpers
{
    public interface IDateTimeFactory
    {
        DateTime UtcNow { get; }
    }
}
