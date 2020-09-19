using System.Collections.Generic;
using System.IO;

namespace ReferenceSequence
{
    public static class ReferenceNameUtilities
    {
        public static Chromosome GetChromosome(Dictionary<string, Chromosome> refNameToChromosome,
            string referenceName)
        {
            if (!refNameToChromosome.TryGetValue(referenceName, out Chromosome chromosome))
                throw new InvalidDataException($"Could not find the following chromosome: {referenceName}");
            
            return chromosome;
        }

        public static Chromosome GetChromosome(Dictionary<ushort, Chromosome> refIndexToChromosome,
            ushort referenceIndex)
        {
            if (!refIndexToChromosome.TryGetValue(referenceIndex, out Chromosome chromosome))
                throw new InvalidDataException(
                    $"Unable to find the reference index ({referenceIndex}) in the refIndexToChromosome dictionary.");

            return chromosome;
        }
    }
}
