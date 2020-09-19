namespace NeoMutalyzer.Annotated
{
    public sealed class Position
    {
        public readonly Variant[] Variants;

        public Position(Variant[] variants) => Variants = variants;
    }
}