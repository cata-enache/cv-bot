using CVBot.Infrastructure.AiConfigurations;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace CVBot.Infrastructure.CvStorage;

public class CvSemanticStoreRepository(
    InMemoryVectorStore vectorStore,
    IOptions<EmbeddingAiModelConfig> embeddingModelConfig) : ICvSemanticStoreWriter, ICvSemanticStoreReader
{
    private const string CvCollectionName = "CvParagraphs";
    private const int TopNResults = 4;

    public async Task SetCvContentAsync(IEnumerable<SemanticCvParagraph> semanticCvParagraphs)
    {
        var collection = GetCvCollectionAsync();

        await collection.EnsureCollectionDeletedAsync();
        await collection.EnsureCollectionExistsAsync();

        await collection.UpsertAsync(semanticCvParagraphs);
    }

    private InMemoryCollection<ulong, SemanticCvParagraph> GetCvCollectionAsync() =>
        vectorStore.GetCollection<ulong, SemanticCvParagraph>(CvCollectionName,
            new VectorStoreCollectionDefinition
            {
                Properties =
                [
                    new VectorStoreVectorProperty(
                        name: nameof(SemanticCvParagraph.Embeddings),
                        dimensions: embeddingModelConfig.Value.EmbeddingsDimension,
                        type: typeof(ReadOnlyMemory<float>)
                    )
                ]
            });

    public async Task<IEnumerable<SemanticCvParagraph>> SearchInCvSemantically(ReadOnlyMemory<float> searchEmbeddings)
    {
        var collection = GetCvCollectionAsync();
        await collection.EnsureCollectionExistsAsync();

        return collection.SearchAsync(searchEmbeddings, TopNResults).ToBlockingEnumerable().Select(x => x.Record);
    }
}