using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using CVBot.Application.CvIngestion;
using CVBot.Domain;
using CVBot.Domain.CvContext;
using CVBot.Domain.Intelligence;
using CVBot.Infrastructure.AiConfigurations;
using CVBot.Infrastructure.CvIngestion;
using CVBot.Infrastructure.CvRetrieval;
using CVBot.Infrastructure.CvStorage;
using CVBot.Infrastructure.Intelligence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Newtonsoft.Json.Linq;
using OpenAI;

namespace CVBot.Infrastructure;

public static class ServiceCollectionExtenstion
{
    private const string MissingRequiredConfigurationGenericMessage = "Missing required configuration:";
    private const string MalformedRequiredConfigurationGenericMessage = "Malformed required configuration:";

    [Experimental("SKEXP0010")]
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterLanguageModels(configuration);
        services.RegisterDomainServices();
        services.RegisterApplicationServices();
        services.RegisterInfrastructureServices();

        return services;
    }
    [Experimental("SKEXP0010")]
    private static IServiceCollection RegisterLanguageModels(this IServiceCollection services, IConfiguration configuration)
    {
        var modelConfigs = configuration.GetRequiredSection(nameof(AiConfig)).Get<AiConfig>() ??
                           throw new Exception(
                               $"{MissingRequiredConfigurationGenericMessage} '{nameof(AiConfig)}'");

        var kernel = services.AddKernel();


        kernel
            .RegisterLanguageModel(modelConfigs.ReasoningAiModel, nameof(modelConfigs.ReasoningAiModel))
            .RegisterEmbeddingModel(modelConfigs.EmbeddingAiModel, nameof(modelConfigs.EmbeddingAiModel));

        return services;
    }


    private static IKernelBuilder RegisterLanguageModel(this IKernelBuilder services,
        LanguageAiModelConfig aiModelConfig, string purpose)
    {
        return aiModelConfig.Provider switch
        {
            AiProvider.Ollama => UseOllamaInference(services, aiModelConfig, purpose),
            AiProvider.AzureOpenAi => UseAzureOpenAiInference(services, aiModelConfig, purpose),
            AiProvider.Groq => UseOpenAiCompatibleInference(services, aiModelConfig, purpose),
            _ => throw new NotImplementedException(),
        };
    }

    private static IKernelBuilder UseAzureOpenAiInference(IKernelBuilder services, LanguageAiModelConfig aiModelConfig,
        string purpose)
    {
        if (string.IsNullOrEmpty(aiModelConfig.DeploymentName))
            throw new Exception(
                $"{MissingRequiredConfigurationGenericMessage}: '{nameof(aiModelConfig.DeploymentName)}'");

        return services.AddAzureOpenAIChatCompletion(
            deploymentName: aiModelConfig.DeploymentName,
            endpoint: aiModelConfig.Endpoint,
            apiKey: aiModelConfig.ApiKey,
            serviceId: purpose);
    }


    private static IKernelBuilder UseOllamaInference(IKernelBuilder services, LanguageAiModelConfig aiModelConfig,
        string purpose)
    {
        return services.AddOllamaChatCompletion(aiModelConfig.ModelName,
            new Uri(aiModelConfig.Endpoint),
            serviceId: purpose);
    }


    private static IKernelBuilder UseOpenAiCompatibleInference(IKernelBuilder services,
        LanguageAiModelConfig aiModelConfig, string purpose)
    {
        var apiKeyCredential = new ApiKeyCredential(aiModelConfig.ApiKey);
        var openAiClientOptions = new OpenAIClientOptions
            { Endpoint = new Uri(aiModelConfig.Endpoint), NetworkTimeout = TimeSpan.FromMinutes(3) };
        var openAiClient = new OpenAIClient(apiKeyCredential, openAiClientOptions);

        return services.AddOpenAIChatCompletion(aiModelConfig.ModelName,
            openAIClient: openAiClient,
            serviceId: purpose);
    }


    [Experimental("SKEXP0010")]
    private static IKernelBuilder RegisterEmbeddingModel(this IKernelBuilder services,
        EmbeddingAiModelConfig aiModelConfig,
        string purpose)
    {
        return aiModelConfig.Provider switch
        {
            AiProvider.Ollama => UseOllamaEmbedding(services, aiModelConfig, purpose),
            AiProvider.AzureOpenAi => UseAzureOpenAiEmbedding(services, aiModelConfig, purpose),
            AiProvider.Groq => UseOpenAiCompatibleEmbedding(services, aiModelConfig, purpose),
            _ => throw new NotImplementedException(),
        };
    }

    [Experimental("SKEXP0010")]
    private static IKernelBuilder UseOpenAiCompatibleEmbedding(IKernelBuilder services,
        EmbeddingAiModelConfig aiModelConfig, string purpose)
    {
        var apiKeyCredential = new ApiKeyCredential(aiModelConfig.ApiKey);
        var openAiClientOptions = new OpenAIClientOptions
            { Endpoint = new Uri(aiModelConfig.Endpoint), NetworkTimeout = TimeSpan.FromMinutes(3) };
        var openAiClient = new OpenAIClient(apiKeyCredential, openAiClientOptions);

        return services.AddOpenAIEmbeddingGenerator(aiModelConfig.ModelName,
            openAIClient: openAiClient,
            serviceId: purpose);
    }

    [Experimental("SKEXP0010")]
    private static IKernelBuilder UseAzureOpenAiEmbedding(IKernelBuilder services, EmbeddingAiModelConfig aiModelConfig,
        string purpose)
    {
        if (string.IsNullOrEmpty(aiModelConfig.DeploymentName))
            throw new Exception(
                $"{MissingRequiredConfigurationGenericMessage}: {purpose} -> '{nameof(aiModelConfig.DeploymentName)}'");

        return services.AddAzureOpenAIEmbeddingGenerator(
            deploymentName: aiModelConfig.DeploymentName,
            endpoint: aiModelConfig.Endpoint,
            apiKey: aiModelConfig.ApiKey, purpose);
    }

    private static IKernelBuilder UseOllamaEmbedding(IKernelBuilder services, EmbeddingAiModelConfig aiModelConfig,
        string purpose)
    {
        return services.AddOllamaEmbeddingGenerator(aiModelConfig.ModelName, 
            new Uri(aiModelConfig.Endpoint),
            serviceId: purpose);
    }

    private static IServiceCollection RegisterDomainServices(this IServiceCollection services)
    {
        services.AddTransient<ICvAssistant, CvAssistant>();
        services.AddTransient<IReasoningModel, ReasoningModel>();
        
        return services;
    }

    private static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ICvIngestor, CvIngestor>();
        
        return services;
    }

    private static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryVectorStore>();
        services.AddTransient<CvSemanticStoreRepository>();
        services.AddTransient<ICvSemanticStoreWriter>(provider => provider.GetRequiredService<CvSemanticStoreRepository>());
        services.AddTransient<ICvSemanticStoreReader>(provider => provider.GetRequiredService<CvSemanticStoreRepository>());
        services.AddTransient<ICvContextRetriever, CvContextRetriever>();
        
        return services;
    }
}