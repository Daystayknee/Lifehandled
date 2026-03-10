using Lifehandled.Application.Ports;
using Lifehandled.Domain.Character;
using Lifehandled.Domain.Common;

namespace Lifehandled.Infrastructure.Genetics
{
    /// <summary>
    /// Minimal founder genetics stub for early new-game flow.
    /// Uses neutral defaults that are safe for prototype balancing.
    /// </summary>
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
                    new GeneValue { geneId = GeneticGeneIds.SleepRecoveryEfficiency, value = 0.5f },
                    new GeneValue { geneId = GeneticGeneIds.StressSensitivity, value = 0.5f }
                }
            };
        }
    }
}
