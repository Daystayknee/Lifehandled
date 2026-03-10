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

            var modifiers = new GeneticModifierProfile
            {
                hungerDecayMultiplier = ConvertGeneToMultiplier(ReadGene(profile, GeneticGeneIds.MetabolismRate)),
                thirstDecayMultiplier = ConvertGeneToMultiplier(ReadGene(profile, GeneticGeneIds.MetabolismRate)),
                sleepRecoveryMultiplier = ConvertGeneToMultiplier(ReadGene(profile, GeneticGeneIds.SleepRecoveryEfficiency)),
                stressGainMultiplier = ConvertGeneToMultiplier(ReadGene(profile, GeneticGeneIds.StressSensitivity))
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
    }
}
