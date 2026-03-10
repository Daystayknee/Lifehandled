using Lifehandled.Application.Session;

namespace Lifehandled.Application.UseCases.World
{
    public static class CalendarRuntimeHelper
    {
        public const int DaysPerWeek = 7;
        public const int DaysPerMonth = 30;
        public const int MonthsPerYear = 12;

        private static readonly string[] DayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        private static readonly string[] MonthNames =
        {
            "Springrise", "Bloomtide", "Suncrest", "Heatfall", "Harvestwane", "Leafdrift",
            "Frostcall", "Snowdeep", "Dawnhold", "Rainbreak", "Riverwake", "Yearsend"
        };

        public static void PopulateCalendar(GameSessionContext context)
        {
            if (context == null)
            {
                return;
            }

            var safeDay = context.currentDay < 1 ? 1 : context.currentDay;
            context.dayOfWeekIndex = ((safeDay - 1) % DaysPerWeek) + 1;
            context.dayOfWeekName = DayNames[context.dayOfWeekIndex - 1];
            context.isWeekend = context.dayOfWeekIndex >= 6;

            context.monthOfYear = ((safeDay - 1) / DaysPerMonth) % MonthsPerYear + 1;
            context.dayOfMonth = ((safeDay - 1) % DaysPerMonth) + 1;
            context.monthName = MonthNames[context.monthOfYear - 1];

            context.activeHolidayId = ResolveHolidayTag(context.monthOfYear, context.dayOfMonth);
        }

        public static string ResolveHolidayTag(int monthOfYear, int dayOfMonth)
        {
            if (dayOfMonth == 1) return "founders_day";
            if (monthOfYear == 2 && dayOfMonth == 14) return "hearts_feast";
            if (monthOfYear == 3 && dayOfMonth == 30) return "bloom_fair";
            if (monthOfYear == 5 && dayOfMonth == 15) return "work_honor_day";
            if (monthOfYear == 6 && dayOfMonth == 30) return "midyear_market";
            if (monthOfYear == 8 && dayOfMonth == 20) return "harvest_ceremony";
            if (monthOfYear == 10 && dayOfMonth == 31) return "lantern_night";
            if (monthOfYear == 12 && dayOfMonth >= 28) return "yearsend_festival";
            return "none";
        }
    }
}
