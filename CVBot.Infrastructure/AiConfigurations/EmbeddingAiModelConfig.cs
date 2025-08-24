namespace CVBot.Infrastructure.AiConfigurations;

public class EmbeddingAiModelConfig :AiModelConfig
{
    public required int MaxTokensPerLine { get; init; }
    public required int MaxTokensPerParagraph { get; init; }
    public required int EmbeddingsDimension { get; init; }
}