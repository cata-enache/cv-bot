namespace CVBot.Infrastructure.AiConfigurations;

public class AiModelConfig
{
    
    public required AiProvider Provider { get; init; }
    
    public required string ModelName { get; init; }

    public required string ApiKey { get; init; }

    public required string Endpoint { get; init; }
    
    public string? DeploymentName { get; init; }
}