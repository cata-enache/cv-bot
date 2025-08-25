using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using CVBot.Application.CvIngestion;
using CVBot.Infrastructure.AiConfigurations;
using CVBot.Infrastructure.CvStorage;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Text;

namespace CVBot.Infrastructure.CvIngestion;

public class CvIngestor(
    ICvSemanticStoreWriter cvSemanticStoreWriter,
    IOptions<EmbeddingAiModelConfig> embeddingAiModelConfig,
    [FromKeyedServices(nameof(AiConfig.EmbeddingAiModel))]
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    ILogger<CvIngestor> logger) : ICvIngestor
{
    private const string GenericFailureMessage = "Failed to ingest cv!";

    [Experimental("SKEXP0050")]
    public async Task<Result> IngestAsync(string cvText)
    {
        try
        {
            var lines = TextChunker.SplitPlainTextLines(cvText,
                maxTokensPerLine: embeddingAiModelConfig.Value.MaxTokensPerLine);

            var paragraphs = TextChunker.SplitPlainTextParagraphs(lines,
                maxTokensPerParagraph: embeddingAiModelConfig.Value.MaxTokensPerParagraph);

            var embeddedParagraphs = await embeddingGenerator.GenerateAsync(paragraphs,
                new EmbeddingGenerationOptions() { Dimensions = embeddingAiModelConfig.Value.EmbeddingsDimension });

            var semanticParagraphs = paragraphs.Zip(embeddedParagraphs).Select((zippedParagraph, index) =>
                new SemanticCvParagraph( (ulong) index, zippedParagraph.First, zippedParagraph.Second.Vector));

            await cvSemanticStoreWriter.SetCvContentAsync(semanticParagraphs);

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, GenericFailureMessage);
            return Result.Failure(GenericFailureMessage);
        }
    }
}