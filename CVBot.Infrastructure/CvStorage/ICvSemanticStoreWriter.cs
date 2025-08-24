namespace CVBot.Infrastructure.CvStorage;

public interface ICvSemanticStoreWriter
{
    Task SetCvContentAsync(IEnumerable<SemanticCvParagraph> semanticParagraphs);
}