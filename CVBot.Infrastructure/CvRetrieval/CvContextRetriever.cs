using System.Collections.Immutable;
using CVBot.Domain.CvContext;
using CVBot.Infrastructure.AiConfigurations;
using CVBot.Infrastructure.CvStorage;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace CVBot.Infrastructure.CvRetrieval;

public class CvContextRetriever(
    ICvSemanticStoreReader cvSemanticStoreReader,
    [FromKeyedServices(nameof(AiConfig.EmbeddingAiModel))]
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : ICvContextRetriever
{
    public async Task<IReadOnlyCollection<CvContextChunk>> GetRelevantContextAsync(string query)
    {
        var queryEmbeddings = await embeddingGenerator.GenerateAsync([query]);
        var results = await cvSemanticStoreReader.SearchInCvSemantically(queryEmbeddings);
        
        return results.Select(x => new CvContextChunk(x.Content))
            .ToImmutableList();
    }
}