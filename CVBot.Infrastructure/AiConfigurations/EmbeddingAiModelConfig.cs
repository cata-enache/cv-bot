namespace CVBot.Infrastructure.AiConfigurations;

public class EmbeddingAiModelConfig :AiModelConfig
{
    public required int MaxBatchElementsCount { get; init; }
    public required int MaxBatchTokenCount { get; init; }
    public required int EmbeddingsDimension { get; init; }
}