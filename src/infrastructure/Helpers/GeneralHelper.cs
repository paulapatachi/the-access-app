using MyApp.Domain.Helpers;

namespace MyApp.Infrastructure.Helpers;

public static class GeneralHelper
{
    public static Tuple<DateTime, DateTime> GetDatesFromRange(string dateRange)
    {
        var dates = dateRange.Split('-');
        var startDate = dates[0].GetDate(); 
        var endDate = dates.Length == 2 ? dates[1].GetDate() : startDate.AddDays(1);

        return new Tuple<DateTime, DateTime>(startDate, endDate);
    }
}