using Microsoft.Extensions.AI;

namespace CVBot.Infrastructure.CvStorage;

public interface ICvSemanticStoreReader
{
    public Task<IEnumerable<SemanticCvParagraph>> SearchInCvSemantically(ReadOnlyMemory<float> searchEmbeddings);
}