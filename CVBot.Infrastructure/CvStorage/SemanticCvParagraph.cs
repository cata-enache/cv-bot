using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace CVBot.Infrastructure.CvStorage;

public class SemanticCvParagraph
{
    [VectorStoreKey]
    public ulong Key { get; set; }
    
    [VectorStoreData]
    public string Content { get; set; } = string.Empty;
    
    public ReadOnlyMemory<float> Embeddings { get; set; }
    
    public SemanticCvParagraph() { }
    
    public SemanticCvParagraph(ulong key, string content, ReadOnlyMemory<float> embeddings)
    {
        Key = key;
        Content = content;
        Embeddings = embeddings;
    }
}