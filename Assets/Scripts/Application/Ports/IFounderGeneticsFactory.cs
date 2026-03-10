using Lifehandled.Domain.Character;

namespace Lifehandled.Application.Ports
{
    /// <summary>
    /// Creates starter founder genetics for new-game characters.
    /// </summary>
    public interface IFounderGeneticsFactory
    {
        GeneticProfile CreateFounderGenetics();
    }
}
