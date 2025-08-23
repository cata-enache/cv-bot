using CVBot.Domain.CvContext;

namespace CVBot.Domain;

public record CvAssistantAnswer(string Answer, IReadOnlyCollection<CvContextChunk> Sources);