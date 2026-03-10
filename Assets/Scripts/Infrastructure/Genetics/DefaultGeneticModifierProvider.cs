using System.Linq;
using Lifehandled.Application.Ports;
using Lifehandled.Domain.Character;

namespace Lifehandled.Infrastructure.Genetics
{
    /// <summary>
    /// Default V1 genetics mapping: normalized gene values (0..1) -> clamped multipliers.
    /// </summary>
    public class DefaultGeneticModifierProvider : IGeneticModifierProvider
    {
        public GeneticModifierProfile BuildModifiers(GeneticProfile profile)
        {
            profile ??= new GeneticProfile();

            var metabolismGene = ReadGene(profile, GeneticGeneIds.MetabolismRate);
            var staminaGene = ReadGene(profile, GeneticGeneIds.StaminaEfficiency);
            var sleepGene = ReadGene(profile, GeneticGeneIds.SleepRecoveryEfficiency);
            var stressGene = ReadGene(profile, GeneticGeneIds.StressSensitivity);
            var illnessGene = ReadGene(profile, GeneticGeneIds.IllnessVulnerability);

            var modifiers = new GeneticModifierProfile
            {
                metabolismMultiplier = ConvertGeneToMultiplier(metabolismGene),
                hungerDecayMultiplier = ConvertGeneToMultiplier(metabolismGene),
                thirstDecayMultiplier = ConvertGeneToMultiplier(metabolismGene),
                energyDecayMultiplier = ConvertInverseGeneToMultiplier(staminaGene),
                staminaRecoveryMultiplier = ConvertGeneToMultiplier(staminaGene),
                sleepRecoveryMultiplier = ConvertGeneToMultiplier(sleepGene),
                sleepQualityMultiplier = ConvertGeneToMultiplier(sleepGene),
                stressGainMultiplier = ConvertGeneToMultiplier(stressGene),
                moodDropMultiplier = ConvertGeneToMultiplier(stressGene),
                illnessRiskGainMultiplier = ConvertGeneToMultiplier(illnessGene)
            };

            return modifiers;
        }

        private static float ReadGene(GeneticProfile profile, string geneId)
        {
            var gene = profile.geneValues.FirstOrDefault(x => x.geneId == geneId);
            return gene?.value ?? 0.5f;
        }

        private static float ConvertGeneToMultiplier(float normalizedGene)
        {
            // 0..1 -> 0.9..1.1 (prototype-safe range)
            var clamped = normalizedGene < 0f ? 0f : (normalizedGene > 1f ? 1f : normalizedGene);
            return 0.9f + (0.2f * clamped);
        }

        private static float ConvertInverseGeneToMultiplier(float normalizedGene)
        {
            // Higher stamina gene should reduce drain.
            var clamped = normalizedGene < 0f ? 0f : (normalizedGene > 1f ? 1f : normalizedGene);
            return 1.1f - (0.2f * clamped);
        }
    }
}
