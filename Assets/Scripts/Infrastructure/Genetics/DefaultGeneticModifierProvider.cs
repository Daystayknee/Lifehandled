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
            var immuneGene = ReadGene(profile, GeneticGeneIds.ImmuneSystemStrength);
            var painGene = ReadGene(profile, GeneticGeneIds.PainTolerance);
            var sleepQualityGene = ReadGene(profile, GeneticGeneIds.SleepQualityTendency);
            var stressToleranceGene = ReadGene(profile, GeneticGeneIds.StressTolerance);
            var agingGene = ReadGene(profile, GeneticGeneIds.AgingRate);
            var hairGrowthGene = ReadGene(profile, GeneticGeneIds.HairGrowthSpeed);

            var modifiers = new GeneticModifierProfile
            {
                metabolismMultiplier = ConvertGeneToMultiplier(metabolismGene),
                hungerDecayMultiplier = ConvertGeneToMultiplier(metabolismGene),
                thirstDecayMultiplier = ConvertGeneToMultiplier(metabolismGene),
                energyDecayMultiplier = ConvertInverseGeneToMultiplier(staminaGene),
                staminaRecoveryMultiplier = ConvertGeneToMultiplier(staminaGene),
                staminaCapacityMultiplier = ConvertGeneToMultiplier(staminaGene),
                sleepRecoveryMultiplier = ConvertGeneToMultiplier(sleepGene),
                sleepQualityMultiplier = ConvertGeneToMultiplier((sleepGene + sleepQualityGene) * 0.5f),
                stressGainMultiplier = ConvertInverseGeneToMultiplier((stressGene + stressToleranceGene) * 0.5f),
                stressToleranceMultiplier = ConvertInverseGeneToMultiplier(stressToleranceGene),
                moodDropMultiplier = ConvertGeneToMultiplier(stressGene),
                immuneStrengthMultiplier = ConvertGeneToMultiplier(immuneGene),
                illnessRiskGainMultiplier = ConvertGeneToMultiplier(illnessGene) * ConvertInverseGeneToMultiplier(immuneGene),
                painToleranceMultiplier = ConvertGeneToMultiplier(painGene),
                agingRateMultiplier = ConvertInverseGeneToMultiplier(agingGene),
                hairGrowthSpeedMultiplier = ConvertGeneToMultiplier(hairGrowthGene)
            };

            return modifiers;
        }

        private static float ReadGene(GeneticProfile profile, string geneId)
        {
            if (profile.geneValues == null || profile.geneValues.Count == 0)
            {
                return 0.5f;
            }

            var match = profile.geneValues.FirstOrDefault(g => g.geneId == geneId);
            if (match == null)
            {
                return 0.5f;
            }

            return Clamp01(match.value);
        }

        private static float ConvertGeneToMultiplier(float gene)
        {
            // 0 -> 0.8, 0.5 -> 1.0, 1 -> 1.2
            return 0.8f + (Clamp01(gene) * 0.4f);
        }

        private static float ConvertInverseGeneToMultiplier(float gene)
        {
            // 0 -> 1.2, 0.5 -> 1.0, 1 -> 0.8
            return 1.2f - (Clamp01(gene) * 0.4f);
        }

        private static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }
    }
}
