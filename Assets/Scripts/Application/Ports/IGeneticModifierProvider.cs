using Lifehandled.Domain.Character;

namespace Lifehandled.Application.Ports
{
    /// <summary>
    /// Provides gameplay-safe stat/needs multipliers from a character genetic profile.
    /// </summary>
    public interface IGeneticModifierProvider
    {
        GeneticModifierProfile BuildModifiers(GeneticProfile profile);
    }
}
