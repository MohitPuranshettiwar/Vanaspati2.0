namespace Vanaspati2._0.Models
{
    public record AnalysisResult
    {
        public string DiseaseName { get; init; }
        public double Confidence { get; init; }
        public string Description { get; init; }
        public string SuggestedTreatment { get; init; }
        public string ImageUrl { get; init; }
    }
}
