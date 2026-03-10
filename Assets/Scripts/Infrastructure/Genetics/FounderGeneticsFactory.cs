using Lifehandled.Application.Ports;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;

namespace Lifehandled.Infrastructure.Genetics
{
    public class FounderGeneticsFactory : IFounderGeneticsFactory
    {
        public GeneticProfile CreateFounderGenetics()
        {
            return new GeneticProfile
            {
                genomeVersion = 1,
                inheritance = new InheritanceMetadata
                {
                    isFounder = true,
                    generationIndex = 0,
                    geneSource = GeneSourceType.Randomized
                },
                geneValues =
                {
                    new GeneValue { geneId = GeneticGeneIds.MetabolismRate, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.StaminaEfficiency, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.SleepRecoveryEfficiency, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.StressSensitivity, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.IllnessVulnerability, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.ImmuneSystemStrength, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.PainTolerance, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.SleepQualityTendency, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.StressTolerance, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.AgingRate, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.HairGrowthSpeed, value = 0.5f }
                }
            };
        }
    }
}
