using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace CVBot.Infrastructure.CvStorage;

public record SemanticCvParagraph(string Content, Embedding<float> Embedding);