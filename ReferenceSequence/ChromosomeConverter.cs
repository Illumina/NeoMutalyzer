using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReferenceSequence
{
    public class ChromosomeConverter : JsonConverter<Chromosome>
    {
        public static Dictionary<string, Chromosome> RefNameToChromosome;

        public override void WriteJson(JsonWriter writer, Chromosome chromosome, JsonSerializer serializer) =>
            writer.WriteValue(chromosome.UcscName);

        public override Chromosome ReadJson(JsonReader reader, Type objectType, Chromosome existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var ucscReferenceName = (string) reader.Value;
            return ReferenceNameUtilities.GetChromosome(RefNameToChromosome, ucscReferenceName);
        }
    }
}