using CVBot.Infrastructure.AiConfigurations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace CVBot.Infrastructure.CvStorage;

public class CvSemanticStoreRepository(
    InMemoryVectorStore vectorStore,
    IOptions<EmbeddingAiModelConfig> embeddingModelConfig) : ICvSemanticStoreWriter
{
    private const string CvCollectionName = "CvParagraphs";

    public async Task SetCvContentAsync(IEnumerable<SemanticCvParagraph> semanticCvParagraphs)
    {
        var collection = await CreateOrResetCvCollectionAsync();
        await collection.UpsertAsync(semanticCvParagraphs);
    }

    private async Task<InMemoryCollection<ulong, SemanticCvParagraph>> CreateOrResetCvCollectionAsync()
    {
        var collection = vectorStore.GetCollection<ulong, SemanticCvParagraph>(CvCollectionName,
            new VectorStoreCollectionDefinition()
            {
                Properties =
                [
                    new VectorStoreVectorProperty(nameof(SemanticCvParagraph.Embedding),
                        embeddingModelConfig.Value.EmbeddingsDimension)
                ]
            });

        await collection.EnsureCollectionDeletedAsync();
        await collection.EnsureCollectionExistsAsync();
        return collection;
    }
}