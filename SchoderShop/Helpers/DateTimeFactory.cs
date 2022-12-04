using System;

namespace SchoderShop.Helpers
{
    public class DateTimeFactory : IDateTimeFactory
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
