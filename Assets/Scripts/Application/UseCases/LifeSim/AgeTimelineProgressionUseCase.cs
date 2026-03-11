using System.Collections.Generic;
using Lifehandled.Application.Session;
using Lifehandled.Domain.Character;

namespace Lifehandled.Application.UseCases.LifeSim
{
    /// <summary>
    /// Prototype age timeline progression. Advances age once per in-game year
    /// and reports life-stage transitions for player feedback.
    /// </summary>
    public class AgeTimelineProgressionUseCase
    {
        public const int DaysPerYear = AgeStageClassifier.PrototypeDaysPerYear;

        public List<string> ApplyEndOfDayAging(GameSessionContext context)
        {
            var timelineEvents = new List<string>();
            if (context == null)
            {
                return timelineEvents;
            }

            if (context.currentDay % DaysPerYear != 0)
            {
                return timelineEvents;
            }

            ProcessCharacter(context.playerCharacter?.data, timelineEvents, "You");

            if (context.householdMembers != null)
            {
                foreach (var member in context.householdMembers)
                {
                    ProcessCharacter(member?.data, timelineEvents, member?.data?.displayName ?? "Household member");
                }
            }

            return timelineEvents;
        }

        private static void ProcessCharacter(CharacterData data, List<string> timelineEvents, string subjectName)
        {
            if (data == null)
            {
                return;
            }

            var previousStage = data.ageStage;
            var previousSubStage = data.ageSubStage;
            data.ageYears += 1;
            data.RecalculateAgeClassification();

            if (data.ageStage != previousStage)
            {
                timelineEvents.Add($"{subjectName} reached {data.ageStage}-{data.ageSubStage} at age {data.ageYears}.");
                return;
            }

            if (data.ageSubStage != previousSubStage)
            {
                timelineEvents.Add($"{subjectName} progressed to {data.ageStage}-{data.ageSubStage}.");
            }
        }
    }
}
