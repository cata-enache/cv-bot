namespace CVBot.Infrastructure.AiConfigurations;

public class AiConfig
{
    public required LanguageAiModelConfig ReasoningAiModel { get; init; }
    public required EmbeddingAiModelConfig EmbeddingAiModel { get; init; }
}