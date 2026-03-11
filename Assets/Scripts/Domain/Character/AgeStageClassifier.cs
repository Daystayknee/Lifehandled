using Lifehandled.Domain.Common;

namespace Lifehandled.Domain.Character
{
    public static class AgeStageClassifier
    {
        public const int PrototypeDaysPerYear = 30;

        public static void Resolve(int ageYears, out LifeAgeStage stage, out LifeAgeSubStage subStage)
        {
            if (ageYears < 0)
            {
                stage = LifeAgeStage.BornBaby;
                subStage = LifeAgeSubStage.A;
                return;
            }

            if (ageYears <= 0) { stage = LifeAgeStage.BornBaby; subStage = ResolveSubStage(ageYears, 0f, 1f); return; }
            if (ageYears <= 1) { stage = LifeAgeStage.Infant; subStage = ResolveSubStage(ageYears, 0f, 1f); return; }
            if (ageYears <= 3) { stage = LifeAgeStage.Toddler; subStage = ResolveSubStage(ageYears, 1f, 3f); return; }
            if (ageYears <= 9) { stage = LifeAgeStage.Child; subStage = ResolveSubStage(ageYears, 4f, 9f); return; }
            if (ageYears <= 12) { stage = LifeAgeStage.Preteen; subStage = ResolveSubStage(ageYears, 10f, 12f); return; }
            if (ageYears <= 15) { stage = LifeAgeStage.Teen; subStage = ResolveSubStage(ageYears, 13f, 15f); return; }
            if (ageYears <= 19) { stage = LifeAgeStage.AdultTeen; subStage = ResolveSubStage(ageYears, 16f, 19f); return; }
            if (ageYears <= 29) { stage = LifeAgeStage.YoungAdult; subStage = ResolveSubStage(ageYears, 20f, 29f); return; }
            if (ageYears <= 54) { stage = LifeAgeStage.Adult; subStage = ResolveSubStage(ageYears, 30f, 54f); return; }
            if (ageYears <= 69) { stage = LifeAgeStage.OlderAdult; subStage = ResolveSubStage(ageYears, 55f, 69f); return; }
            if (ageYears <= 110) { stage = LifeAgeStage.Elder; subStage = ResolveSubStage(ageYears, 70f, 110f); return; }

            stage = LifeAgeStage.Death;
            subStage = LifeAgeSubStage.E;
        }

        public static bool TryGetNextMajorStage(LifeAgeStage current, out LifeAgeStage next)
        {
            if (current == LifeAgeStage.Death)
            {
                next = LifeAgeStage.Death;
                return false;
            }

            next = (LifeAgeStage)((int)current + 1);
            return true;
        }

        public static int ResolveDaysUntilNextBirthday(int currentDay)
        {
            var remainder = currentDay % PrototypeDaysPerYear;
            return remainder == 0 ? PrototypeDaysPerYear : PrototypeDaysPerYear - remainder;
        }

        private static LifeAgeSubStage ResolveSubStage(float ageYears, float min, float max)
        {
            if (max <= min)
            {
                return LifeAgeSubStage.A;
            }

            var normalized = (ageYears - min) / (max - min);
            if (normalized <= 0.2f) return LifeAgeSubStage.A;
            if (normalized <= 0.4f) return LifeAgeSubStage.B;
            if (normalized <= 0.6f) return LifeAgeSubStage.C;
            if (normalized <= 0.8f) return LifeAgeSubStage.D;
            return LifeAgeSubStage.E;
        }
    }
}
