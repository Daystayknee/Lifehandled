using Lifehandled.Application.Session;
using Lifehandled.Domain.Common;

namespace Lifehandled.Application.UseCases.Economy
{
    /// <summary>
    /// Handles daily income and weekly rent payment.
    /// </summary>
    public class DailyEconomySettlementUseCase
    {
        public string Execute(GameSessionContext context)
        {
            if (context == null)
            {
                return "No session.";
            }

            context.economy ??= new EconomyState();
            var economy = context.economy;

            var incomePaid = 0;
            if (economy.currentJob != JobType.Unemployed && economy.lastIncomePaidDay != context.currentDay)
            {
                incomePaid = economy.dailyIncome;
                context.wallet += incomePaid;
                economy.lastIncomePaidDay = context.currentDay;
            }

            var rentPaid = 0;
            if (context.currentDay % 7 == 0 && economy.lastRentPaidDay != context.currentDay)
            {
                rentPaid = economy.weeklyRentCost;
                context.wallet -= rentPaid;
                economy.lastRentPaidDay = context.currentDay;
            }

            return $"Income +${incomePaid}, rent -${rentPaid}.";
        }
    }
}
