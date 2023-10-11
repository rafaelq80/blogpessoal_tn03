using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace blogpessoal.Configuration
{
    public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
    {

        public DateTimeOffsetConverter()
            : base(
                d => d.ToUniversalTime().ToOffset(TimeZoneInfo.FindSystemTimeZoneById("Brazil/East").GetUtcOffset(d)),
                d => d.ToUniversalTime().ToOffset(TimeZoneInfo.FindSystemTimeZoneById("Brazil/East").GetUtcOffset(d))
            )
        { }
   
    }
}

